require 'nn'

function make_net(opt,max_sent)
	local kernel = opt.kernels
	para = nn.Parallel(1,2)
	for i=1,#kernel do
		local convl = nn.Sequential()
		convl:add(nn.Reshape(1,max_sent, opt.vec_size,true))
		convl:add(nn.SpatialConvolution(1, opt.num_feat_maps, opt.vec_size, kernel[i]))
		convl:add(nn.ReLU())
		convl:add(nn.Max(3))
		convl:add(nn.Reshape(opt.num_feat_maps))
		para:add(convl)
	end
	mlp = nn.Sequential()
	mlp:add(para)
	return mlp
end



function make_complete_net(opt)
	mlp_q = make_net(opt,opt.q_max_sent)
	mlp_spo = make_net(opt,opt.spo_max_sent)
	prl=nn.ParallelTable()
	prl:add(mlp_q)
	prl:add(mlp_spo)
	mlp = nn.Sequential()
	mlp:add(prl)

	criterion = nn.CosineEmbeddingCriterion()
	param,grad_params=mlp:getParameters()
	return mlp, param, grad_params, criterion
end

