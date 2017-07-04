#! /usr/bin/env python

import tensorflow as tf
import numpy as np
import os
import time
import datetime
import data_helpers
import csv
import program
import CandidateGeneration

# Parameters
# ==================================================

# Data Parameters
tf.flags.DEFINE_string("Questions", "./data/QuestionsForTest_1.tsv", "Data source for the questions data.")
tf.flags.DEFINE_string("CandidateTriples", "./data/CandidateTriples.npy", "Ouputting source triples for the question data.")


FLAGS = tf.flags.FLAGS
FLAGS._parse_flags()
print("\nParameters:")
for attr, value in sorted(FLAGS.__flags.items()):
    print("{}={}".format(attr.upper(), value))
print("")

# CHANGE THIS: Load data. Load your own data here
#if FLAGS.eval_train:
	

AllData = CandidateGeneration.LoadAllData()
print AllData.shape

model=program.loadModel()

QuestionsFile = open(FLAGS.Questions,"r")
CandidatesFileWithQuestions = open('./data/CandidateTriplesWithQuestion.csv',"w")
All= np.array([], dtype=np.float64).reshape(0,7,300)

for question in QuestionsFile:	
	if question is not None:
		CandidateTriples = CandidateGeneration.GenerateCandidate(question,AllData,model,CandidatesFileWithQuestions)
		#print CandidateTriples.shape
		SPOEmb = CandidateGeneration.MakeCandidateEmbeddingsPerQuestion(CandidateTriples,model)
		print SPOEmb.shape
		All = np.concatenate((All, SPOEmb),axis=0)		
		#print All.shape

print All.shape
np.save(FLAGS.CandidateTriples, All)
		