function get_layer(model, name)
  local named_layer
  function get(layer)
    if layer.name == name or torch.typename(layer) == name then
      named_layer = layer
    end
  end

  model:apply(get)
  return named_layer
end

function CreateModelInput(opt, q_embedding, spo_embedding)

	local ques_conv_input = torch.Tensor(#opt.kernels,q_embedding:size(1),q_embedding:size(2), q_embedding:size(3));
	local spo_conv_input = torch.Tensor(#opt.kernels, spo_embedding:size(1), spo_embedding:size(2),spo_embedding:size(3));
	
	for i=1,#opt.kernels do
		ques_conv_input[i] = q_embedding;
		spo_conv_input[i]=spo_embedding;
	end
	
	local input = {ques_conv_input,spo_conv_input};
	return input
end

function string:split(sSeparator, nMax, bRegexp)
   assert(sSeparator ~= '')
   assert(nMax == nil or nMax >= 1)
   local aRecord = {}
   if self:len() > 0 then
      local bPlain = not bRegexp
      nMax = nMax or -1
      local nField, nStart = 1, 1
      local nFirst,nLast = self:find(sSeparator, nStart, bPlain)
      while nFirst and nMax ~= 0 do
         aRecord[nField] = self:sub(nStart, nFirst-1)
         nField = nField+1
         nStart = nLast+1
         nFirst,nLast = self:find(sSeparator, nStart, bPlain)
         nMax = nMax-1
      end
      aRecord[nField] = self:sub(nStart)
   end
   return aRecord
end