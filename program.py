import sys
import os
import numpy as np
from gensim.models import Word2Vec
import re

QuestionWordLenght =15
WordVectorLength=300
PredicateWordLength=5
EntityWordLength=1
ObjectWordLength=1

class Vectors:
	def __init__(self,NoOfLines):
		self.VectorsForQuestions = np.empty([NoOfLines,QuestionWordLenght,WordVectorLength])
		self.VectorsForAnswers = np.empty([NoOfLines,PredicateWordLength+EntityWordLength+ObjectWordLength,WordVectorLength])

def Preprocess(string):
    string =string.replace('\n','').replace('\r','').strip()
    string = re.sub(r"[^A-Za-z0-9(),!?\'\`]", " ", string)
    string = re.sub(r"\'s", " \'s", string)
    string = re.sub(r"\'ve", " \'ve", string)
    string = re.sub(r"n\'t", " n\'t", string)
    string = re.sub(r"\'re", " \'re", string)
    string = re.sub(r"\'d", " \'d", string)
    string = re.sub(r"\'ll", " \'ll", string)
    string = re.sub(r",", " , ", string)
    string = re.sub(r"!", " ! ", string)
    string = re.sub(r"\?", " \? ", string)
    string = re.sub(r"\s{2,}", " ", string)
    return string.strip().lower()

def InitializeRandomVector():
	vector = np.zeros([WordVectorLength])
	return vector;

def MakeQuestionVectors(question,model):
	QuestionVector = np.empty([QuestionWordLenght,WordVectorLength])
	wordCount=0
	question = question.replace('\n','').replace('\r','').strip()
	print question
	words = question.split(" ")
	lenWords = len(words)
	if(lenWords > QuestionWordLenght):
		print "Length of words in the question exceed max length of " +str(QuestionWordLenght)
	if(lenWords <= QuestionWordLenght):
		RemainingWordsToFill = QuestionWordLenght - lenWords
		print "Appendind Zero Vector for making the predcate length to" + str(QuestionWordLenght)
		for i in range(lenWords,QuestionWordLenght):
			question = question + " <PAD>"
		words = question.split(" ")
		for word in words:
			if word.count("@") >1:
				formattedWord = word.replace("@","").replace(";","").strip()
				print "Doing wiki2vec: " + formattedWord
				if(formattedWord.find("<PAD>")!=-1):
					QuestionVector[wordCount] = InitializeRandomVector()
				else:
					inputToWiki="DBPEDIA_ID/"+formattedWord
					try:
						vector =model[inputToWiki]
						if vector is None:
							vector=model[formattedWord.lower()]
						if vector is not None:
							QuestionVector[wordCount] = vector
					except Exception:
						print "Exception in wiki2vec in MakeQuestionVectors function: " + inputToWiki
						QuestionVector[wordCount]=InitializeRandomVector()
			else:
				if(word.find("<PAD>")!=-1):
					QuestionVector[wordCount]=InitializeRandomVector()
				else:
					try:
						print "Doing a word2vec: " + word
						vector =model[word.lower()]
						QuestionVector[wordCount] = vector
					except Exception:
						print "Exception in word2vec in MakeQuestionVectors function: " + word
						QuestionVector[wordCount]=InitializeRandomVector()
			wordCount = wordCount+1
	return QuestionVector;

def MakeEntityVectors(entity,model):
	EntityVector = np.empty([WordVectorLength])
	print "Making vectors for entities for entity " + entity
	Entity="DBPEDIA_ID/"+entity
	try:
		EntityVector =model[Entity]
		if EntityVector is None:
			EntityVector=model[entity.lower().replace("_","")]
	except Exception:
		print "Exception in MakeEntityVectors function: " + entity
		EntityVector = InitializeRandomVector()
	return EntityVector;

def MakeObjectVectors(object,WikiEntity,model):
	ObjectVector = np.empty([WordVectorLength])
	if(is_number(object)):
		print "Is Number so not making vectors"
		ObjectVector = InitializeRandomVector()
	print "Making Object Vectors for object " + object
	WikiEntitiesSplit=WikiEntity.split(";")
	if not WikiEntitiesSplit:
		print "Doesn't have WikiEntity from TagMe"
		ObjectVector = InitializeRandomVector()
	else:
		try:
			ObjectVector=model["DBPEDIA_ID/"+WikiEntitiesSplit[0]]
			if ObjectVector is None and len(WikiEntitiesSplit) > 1:
				ObjectVector=model["DBPEDIA_ID/"+WikiEntitiesSplit[1]]
		except Exception:
			print "Exception in ObjectVector: "+ WikiEntitiesSplit[0]
			ObjectVector = InitializeRandomVector()
	return ObjectVector

def MakePredicateVectors(predicate,model):
	print "Making vectors for predicate: " + predicate
	PredicateVector = np.empty([PredicateWordLength,WordVectorLength])
	wordCount=0
	words = predicate.split(" ")
	lenWords = len(words)
	if( lenWords > PredicateWordLength):
		print "Length of words in the predicate exceed max length of" + str(PredicateWordLength)
	if(lenWords <= PredicateWordLength):
		RemainingWordsToFill = PredicateWordLength - lenWords
		print "Appendind <PAD> for making the predcate length to "+ str(PredicateWordLength)
		for i in range(lenWords,PredicateWordLength):
			predicate = predicate + " <PAD>"
		words = predicate.split(" ")
		for word in words:
			formattedWord = word.lower()
			print "Doing word2vec: " + formattedWord
			if(formattedWord.find("<pad>")!=-1):
				PredicateVector[wordCount]= InitializeRandomVector()
			try:
				PredicateVector[wordCount] =model[formattedWord]
			except Exception:
				print "Exception in word2vec in MakePredicateVectors function: " + predicate
				PredicateVector[wordCount]= InitializeRandomVector()
			wordCount = wordCount+1
	return PredicateVector;
	
def is_number(var):
	try:
		if var==int(var):
			return False
		else:
			return True
	except Exception:
		return False

def loadData(inputdata,y):
	outputfile = open("./QAEmbeddings.tsv","w")
	labelsfile = open("./labels.tsv","w")
	modelPath = os.path.join("/mnt/c/Wiki2vec/output16_6/","model.w2c")
	model = Word2Vec.load(modelPath)
	print "Making Vectors for Input data..."
	linenumber = 0
	np.set_printoptions(threshold=np.inf)
	
	#NumberOfLines = sum(1 for line in inputfile)
	vecs = Vectors(len(inputdata))
	for line in inputdata:
		wordCount = 0
		line = line.rstrip("\n").rstrip("\r")
		cols = line.split("\t")
		if(len(cols)<8):
			print "Number of columns invalid in the line"
		elif(len(cols)==8):
			EntityVector= MakeEntityVectors(cols[4],model) # passing DBPedia Subject
			ObjectVector= MakeObjectVectors(cols[2],cols[5],model) # passing object and dbpedia object
			PredicateVector = MakePredicateVectors(cols[1],model)
			QuestionVector = MakeQuestionVectors(cols[7],model)		
			if EntityVector is not None and ObjectVector is not None and PredicateVector is not None and QuestionVector is not None:
				EntityVector = EntityVector.reshape(EntityWordLength,WordVectorLength)
				ObjectVector = ObjectVector.reshape(ObjectWordLength,WordVectorLength)
				vecs.VectorsForQuestions[linenumber] = QuestionVector
				vecs.VectorsForAnswers[linenumber] = np.concatenate((EntityVector,ObjectVector,PredicateVector),axis=0)
				OneArray = np.concatenate((QuestionVector,EntityVector,ObjectVector,PredicateVector),axis=0)
				print OneArray.shape
				outputfile.write(str(OneArray)+"\n")
                labelsfile.write(str(y[linenumber])+"\n")	
                print "Writing to file: " + str(linenumber)
                linenumber=linenumber +1	
			#else:
			#	print "Some vector was null"
        else:
            print "Number of columns exceed"
	outputfile.close();
	labelsfile.close()
	return vecs.VectorsForQuestions,vecs.VectorsForAnswers