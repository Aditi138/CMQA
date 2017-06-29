#CandidateGeneration
import os
import numpy as np

def GenerateCandidate(question):
	ifile = os.path.join("/mnt/d/codeMix/QT Pairs/","AllData_1.tsv")
	AllData = open(ifile,"r")

	print "Loading AllData file..."

	Candidates=[]

	for line in AllData:
		line = line.rstrip("\n").rstrip("\r")
		cols = line.split("\t")
		SPO =[]
		if(cols is None and len(cols)!=8):
			print "Number of cols in invalid"
		else:
			Entity=cols[0]
			Predicate=cols[1]
			Object=cols[2]
			if(ContainsSPO(question,Entity,Object)):
				SPO.append(Entity)
				SPO.append(Predicate)
				SPO.append(Object)
				SPO.append(cols[4])
				SPO.append(cols[5])
				Candidates.append(np.array(SPO))
			#print Candidates[i]
			#i =i+1
	return Candidates;

def ContainsSPO(question, entity,object):
	question = question.lower()
	entity = entity.rstrip().lower()
	object =object.rstrip().lower()
	if(question.find(entity) or question.find(object)):
		return True
	else:
		Entitylist = entity.split(' ')
		if(Entitylist is not None and len(Entitylist) > 0):
			sb=""
			for e in Entitylist:
				en = e.strip().replace('(','').replace(')','')
				sb = sb + en			
				if(question.find(en) or question.find(sb)):
					return True
		Objectlist = object.split(' ')
		if(Objectlist is not None and len(Objectlist) > 0):
			obj=""
			for o in Objectlist:
				ob = o.strip().replace('(','').replace(')','')
				obj=obj+ob
				if(question.find(ob) or question.find(obj)):
					return True
	return False
	
		
	