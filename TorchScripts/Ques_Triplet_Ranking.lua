require 'nn'
require 'util.lua'
require 'csvigo'
require 'npy4th';


best_model=torch.load('best_model');
opt = torch.load('opt');

pattern1 = '##'
pattern2 = '||'

K = 10 -- top ranked entities


cos = nn.CosineDistance();

function load_questio_triple_pair()
	npy4th = require 'npy4th';
	question_embedding = npy4th.loadnpy('data/codemixdata/testing/QuestionEmbeddings.npy');
	triplet_embedding = npy4th.loadnpy('data/codemixdata/testing/CandidateTriples.npy');
	CandidateTriplesWithQuestion = csvigo.load{path='data/codemixdata/testing/CandidateTriplesWithQuestion.csv',mode='raw'};
	
	return question_embedding, triplet_embedding,CandidateTriplesWithQuestion
end

function GetTopRankedEntities(predScore)
	res, ind = torch.sort(predScore,true)
	res = res[{{1,K}}]
	ind = ind[{{1,K}}]
	return res, ind
end

function FormQuestionRankedTripletAnswer(qestion, triplets, ranked_index)
	local result = {}
	result[1] = qestion;
	print(result[1])
	local j=2
	for i=1,K do
		result[j] = triplets[ranked_index[i]]
		j = j+1
	end
	return result
end

function GenerateQuestionRankedTriplets(question_embedding, triplet_embedding, CandidateTriplesWithQuestion)
	local QE_Size = question_embedding:size(1);
	local CTWQ_Size = #CandidateTriplesWithQuestion;
	local Result_Question_Triples = {};
	local Ranked_Entries = torch.Tensor(QE_Size,K);
	print('triplet embed_size ', (#triplet_embedding)[1])
	local dis
	local embed_size =0;
	for i=1,QE_Size do
		print('Question --> ',i)
		-- get the question and triplets in raw from
		local ques_triplets = CandidateTriplesWithQuestion[i][1];
		ques_triplets = string.split(ques_triplets,pattern1);
		local question = ques_triplets[1];
		local triplets = string.split(ques_triplets[2],pattern2);
		
		
		
		local M = #triplets - 1;  -- number of candidate triplets for qestion i
		print("M size",M);
		print("embed_size value ",embed_size)
		local ques_repeat_emd = torch.repeatTensor(question_embedding[i],M,1,1);
		--print ('ques embed_size ',#ques_repeat_emd)
		local spo_embedding = triplet_embedding[{{embed_size+1, embed_size+M},{},{}}];
		--print ('spo embed_size ',#spo_embedding)
		local input = CreateModelInput(opt,ques_repeat_emd,spo_embedding);
		--print('input size', #input);
		local pred = best_model:forward(input);
		--print('pred size', #pred);top
		local p1 = pred[1]:double()
		print('p1 size', #p1);
		local p2 = pred[2]:double()
		print('p2 size', #p2);
		dis = cos:forward({p1,p2})
		print('dis size ',#dis)
		local res, ind = GetTopRankedEntities(dis);
		print(res)
		print(ind)
		Ranked_Entries[i]=ind;
		local result = FormQuestionRankedTripletAnswer(question,triplets,ind);
		table.insert(Result_Question_Triples,result);
		embed_size = embed_size+M;
		
	end
	return dis,Ranked_Entries, Result_Question_Triples
end

function dummy()
	local question_embedding, triplet_embedding, CandidateTriplesWithQuestion = load_questio_triple_pair()
	local dis,ranked_entities, Result_Question_Triples = GenerateQuestionRankedTriplets(question_embedding, triplet_embedding, CandidateTriplesWithQuestion)
	return dis, ranked_entities, Result_Question_Triples
end