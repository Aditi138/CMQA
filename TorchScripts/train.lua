require 'convNN_new_1.lua'
require 'nn'
require 'util.lua'

colour = require 'trepl.colorize'

function train_data(q_embedding,spo_embedding, train_label, model, criterion, param, gred)

	local total_err = 0
	local train_size = (#q_embedding)[1]
	local num_batches = math.floor(train_size / opt.batch_size);
	local shuffle = torch.randperm(num_batches)
	for i = 1, shuffle:size(1) do
		local t = (shuffle[i] - 1) * opt.batch_size + 1
		local batch_size = math.min(opt.batch_size, train_size - t + 1)

		--print('t: ',t,' batch size: ',batch_size);
		--data samples
		local target = train_label:narrow(1,t,batch_size);
		local train_ques = q_embedding:narrow(1,t,batch_size);
		local train_spo = spo_embedding:narrow(1,t,batch_size);

		--print('train ques size: ',#train_ques)
		--print('train spo size: ',#train_spo)		
		--print('train label size: ',#train_label)
		--forward pass
		local input = CreateModelInput(opt,train_ques,train_spo)
		local output = model:forward(input)
		
		--print('output size: ',#output)
		local err = criterion:forward(output,target)

		-- track error and confusion
		total_err = total_err + err*batch_size
		
		--Compute Gradient
		grad = criterion:backward(output,target)
		model:backward(input,grad)
		model:updateParameters(0.01)
	end
	print(colour.red('loss: '), total)
	return total_err
end


function test_error(q_test, spo_test,test_label, model,criterion)
	local test =  CreateModelInput(opt,q_test,spo_test)
	local output = model:forward(test,test_label)
	local test_err = criterion:forward(output,test_label)
	return test_err
end


function train_loop(all_q_embedding, all_spo_embedding,all_train_label,opt)


	
	--print('all q embedding size', all_q_embedding:size(1))
	--print('all spo embedding size',all_spo_embedding:size(1))
	--print('all train label size', all_train_label:size(1))
	
	local q_train = all_q_embedding
	local spo_train = all_spo_embedding
	local train_label = train_label
	
	local model, param, grad_params, criterion = make_complete_net(opt);
	
	local best_model=model:clone()
	--Training Folds
	for fold =1,opt.folds do
		print()
		print('==> fold ', fold)

		if opt.has_test ==0 then
			-- make train/test data (90/10 split for train/test)
			local N = q_embedding:size(1)
			local i_start = math.floor((fold - 1) * (N / opt.folds) + 1)
			local i_end = math.floor(fold * (N / opt.folds))
			-- test data
			test_q_embedding = all_q_embedding:narrow(1, i_start, i_end - i_start + 1)
			test_spo_embedding = all_spo_embedding:narrow(1, i_start, i_end - i_start + 1)
			test_label = all_train_label:narrow(1, i_start, i_end - i_start + 1)
			 -- train data
			q_train = torch.cat(all_q_embedding:narrow(1, 1, i_start), all_q_embedding:narrow(1, i_end, N - i_end + 1), 1)
			spo_train = torch.cat(all_spo_embedding:narrow(1, 1, i_start), all_spo_embedding:narrow(1, i_end, N - i_end + 1), 1)
			train_label = torch.cat(all_train_label:narrow(1, 1, i_start), all_train_label:narrow(1, i_end, N - i_end + 1), 1)
			
		end
			
		if opt.has_dev ==0 then
			local J = q_train:size(1)
			local shuffle = torch.randperm(J):long()
			q_train = q_train:index(1, shuffle)
			spo_train = spo_train:index(1,shuffle)
			train_label = train_label:index(1, shuffle)

			local num_batches = math.floor(J / opt.batch_size)
			--print ('num of batches:',num_batches)
			local num_train_batches = torch.round(num_batches * 0.9)
			--print ('num of train batches', num_train_batches)
			local train_size = num_train_batches * opt.batch_size
			local dev_size = J - train_size
			q_dev = q_train:narrow(1, train_size+1, dev_size)
			spo_dev = spo_train:narrow(1, train_size+1, dev_size)
			dev_label = train_label:narrow(1, train_size+1, dev_size)
			--print('train size:',train_size)
			q_train = q_train:narrow(1, 1, train_size)
			spo_train = spo_train:narrow(1, 1, train_size)
			train_label = train_label:narrow(1, 1, train_size)
		end

		
	
		
		--training loop
		local best_epoch =1
		local best_err =1
		local epoch = 1
		
		repeat
				local train_err = train_data(q_train,spo_train, train_label, model, criterion, param, grad_params)
				local dev_error = test_error(q_dev,spo_dev,dev_label,model,criterion)
				print ('epoch: ',epoch,' train error: ',train_err,' dev error: ',dev_error, 'best error:', best_err);
				if(dev_error < best_err) then
					best_model=model:clone()
					best_err=dev_error
					best_epoch=epoch
				end
				epoch = epoch+1
				print (dev_error > best_err or epoch>10)
		until dev_error > best_err or epoch>10
				
		local test_err =  test_error(test_q_embedding,test_spo_embedding,test_label,model,criterion)
		print('test error: ',test_err);	
	end -- fold loop ends
	return best_model
end
