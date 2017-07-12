require 'nn';
require 'optim';
require 'lfs';
require 'util.lua';
require 'convNN_new_1.lua'
require 'train.lua'


-- Flags

cmd = torch.CmdLine()

cmd:text()
cmd:text()
cmd:text('Convolutional net for sentence classification')
cmd:text()
cmd:text('Options')
cmd:option('-model_type', 'nonstatic', 'Model type. Options: rand (randomly initialized word embeddings), static (pre-trained embeddings from word2vec, static during learning), nonstatic (pre-trained embeddings, tuned during learning), multichannel (two embedding channels, one static and one nonstatic)')
cmd:option('-data', '', 'Training data and word2vec data')
cmd:option('-cudnn', 0, 'Use cudnn and GPUs if set to 1, otherwise set to 0')
cmd:option('-seed', 3435, 'random seed, set -1 for actual random')
cmd:option('-folds', 4, 'number of folds to use. If test set provided, folds=1. max 10')
cmd:option('-debug', 0, 'print debugging info including timing, confusions')
cmd:option('-gpuid', 0, 'GPU device id to use.')
cmd:option('-savefile', '', 'Name of output file, which will hold the trained model, model parameters, and training scores. Default filename is TIMESTAMP_results')
cmd:option('-zero_indexing', 0, 'If data is zero indexed')
cmd:option('-dump_feature_maps_file', '', 'Set file to dump feature maps of convolution')
cmd:text()



-- Preset by preprocessed data

cmd:option('-has_test', 0, 'If data has test, we use it. Otherwise, we use CV on folds')
cmd:option('-has_dev', 0, 'If data has dev, we use it, otherwise we split from train')
cmd:option('-num_classes', 2, 'Number of output classes')
cmd:option('-max_sent', 22, 'maximum sentence length')
cmd:option('-vec_size', 300, 'word2vec vector size')
cmd:option('-vocab_size', 18766, 'Vocab size')

cmd:text()

-- Training own dataset

cmd:option('-train_only', 0, 'Set to 1 to only train on data. Default is cross-validation')
cmd:option('-test_only', 0, 'Set to 1 to only do testing. Must have a -warm_start_model')
cmd:option('-preds_file', '', 'On test data, write predictions to an output file. Set test_only to 1 to use')
cmd:option('-warm_start_model', '', 'Path to .t7 file with pre-trained model. Should contain a table with key \'model\'')
cmd:text()

-- Training hyperparameters

cmd:option('-num_epochs', 25, 'Number of training epochs')
cmd:option('-optim_method', 'adadelta', 'Gradient descent method. Options: adadelta, adam')
cmd:option('-L2s', 3, 'L2 normalize weights')
cmd:option('-batch_size', 6, 'Batch size for training')
cmd:text()



-- Model hyperparameters

cmd:option('-num_feat_maps', 128, 'Number of feature maps after 1st convolution')
cmd:option('-kernels', {3,4,5}, 'Kernel sizes of convolutions, table format.')
cmd:option('-skip_kernel',0, 'Use skip kernel')
cmd:option('-dropout_p', 0.5, 'p for dropout')
cmd:option('-highway_mlp', 0, 'Number of highway MLP layers')
cmd:option('-highway_conv_layers', 1, 'Number of highway MLP layers')
cmd:option('-q_max_sent',15,'max sentence size of the question embedding')
cmd:option('-spo_max_sent',7,'max sentence size of the <SPO> embedding')

cmd:text()

function load_data()
	npy4th = require 'npy4th';
	local train, train_label
	embedding = npy4th.loadnpy('data/codemixdata/QAEmbeddings.npy');
	target = npy4th.loadnpy('data/codemixdata/labels.npy');
	target = target:resize(target:size(1))
	q_embedding = embedding[{{},{1,opt.q_max_sent},{}}];
	spo_embedding = embedding[{{},{opt.q_max_sent+1,(#embedding)[2]},{}}];
	train_label = torch.Tensor(target:size(1))
	for i=1,target:size(1) do
		train_label[i] = target[i]
	end
	return q_embedding, spo_embedding,train_label
end





function main()
	-- parse arguments
	--  if opt.seed ~= -1 then
	opt = cmd:parse(arg)
	q_embedding,spo_embedding, train_label = load_data()
	
	mlp, param, grad_params, criterion  = make_complete_net(opt);
	best_model = train_loop(q_embedding,spo_embedding,train_label,opt);
	torch.save('best_model',best_model);
	torch.save('opt',opt);
end