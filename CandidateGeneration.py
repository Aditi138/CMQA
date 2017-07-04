#CandidateGeneration
import os
import numpy as np
import program

def LoadAllData():
	ifile = os.path.join("/mnt/d/codeMix/QT Pairs/","AllData_1.tsv")
	AllData = open(ifile,"r")
	SPO =[]
	i=0
	
	print "Loading AllData file..."
	for line in AllData:
		line = line.rstrip("\n").rstrip("\r")
		cols = line.split("\t")
		Triple=[]
		
		if(cols is None and len(cols)!=8):
			print "Number of cols in invalid"
		else:
			Triple.append(cols[0])
			Triple.append(cols[1])
			Triple.append(cols[2])
			Triple.append(cols[4])
			Triple.append(cols[5])
			SPO.append(Triple)
			#print len(SPO[i])
			i=i+1
			
	return np.array(SPO)

def GenerateCandidate(question,AllData,model,CandidatesFileWithQuestions):
	
	Candidates=[]
	dict={}
	question = question.rstrip("\n").rstrip("\r")
	questionCols = question.split("\t")
	if(len(questionCols)==2):
		
		CandidatesFileWithQuestions.write(questionCols[0]+"##")

		for i in xrange(AllData.shape[0]):
			Value =[]
			Entity=AllData[i][0]
			#print Entity
		
			Predicate=AllData[i][1]
			Object=AllData[i][2]
			if(ContainsSPO(question,Entity,Object)):
				dictKey = Entity+"@"+Object+"@"+Predicate
				if(dictKey not in dict):
					dict[dictKey] = 1
					Value.append(Entity)
					Value.append(Predicate)
					Value.append(Object)
					Value.append(AllData[i][3])
					Value.append(AllData[i][4])
					CandidatesFileWithQuestions.write(Entity+"\t"+Predicate+"\t"+Object+"||")
					Candidates.append(Value)
				else:
					print "Already in dict: " + dictKey
		CandidatesFileWithQuestions.write("\n")
		return np.array(Candidates);

def ContainsSPO(question, entity,object):
	question = question.lower()
	entity = entity.rstrip().lower()
	object =object.rstrip().lower()
	if(question.find(entity) > -1 or question.find(object) > -1):
		#print "Found either entity or object"
		return True
	else:
		Entitylist = entity.split(' ')
		if(Entitylist is not None and len(Entitylist) > 0):
			sb=""
			for e in Entitylist:
				en = e.strip().replace('(','').replace(')','').replace('the','')
				sb = sb + en			
				if(question.find(en) > -1 or question.find(sb) > -1):
					#print "Found in part of entity"
					return True
		# Objectlist = object.split(' ')
		# if(Objectlist is not None and len(Objectlist) > 0):
			# obj=""
			# for o in Objectlist:
				# ob = o.strip().replace('(','').replace(')','')
				# obj=obj+ob
				# if(question.find(ob) > -1 or question.find(obj) > -1):
					# return True
	return False

def MakeCandidateEmbeddingsPerQuestion(CandidateTriples,model):
	NumberOfCandidates = CandidateTriples.shape[0]
	Embedding= np.empty([NumberOfCandidates,program.PredicateWordLength+program.EntityWordLength+program.ObjectWordLength,program.WordVectorLength])
	for i in xrange(CandidateTriples.shape[0]):
		Entity = CandidateTriples[i][0]
		Predicate = CandidateTriples[i][1]
		Object = CandidateTriples[i][2]
		DBEntity = CandidateTriples[i][3]
		DBObject = CandidateTriples[i][4]
		Embedding[i] = program.load_Answer_Embeddings(Entity,Object,Predicate, DBEntity, DBObject,model)
	return Embedding
	
		
	