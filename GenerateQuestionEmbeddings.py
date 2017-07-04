import numpy as np
import program
import data_helpers
import tensorflow as tf

# Data Parameters
tf.flags.DEFINE_string("Questions", "./data/QuestionsForTest_1.tsv", "Data source for the questions data.")
tf.flags.DEFINE_string("QuestionEmbeddings", "./data/QuestionEmbeddings.npy", "Ouputting source triples for the question data.")
tf.flags.DEFINE_string("CandidatesGenerated", "./data/CandidatesGenerated.tsv", "Data source for the questions data.")
tf.flags.DEFINE_string("SPoEmbeddings", "./data/TripleEmbeddings.npy", "Ouputting source triples for the question data.")


FLAGS = tf.flags.FLAGS
FLAGS._parse_flags()
print("\nParameters:")
for attr, value in sorted(FLAGS.__flags.items()):
    print("{}={}".format(attr.upper(), value))
print("")
model=program.loadModel()
ProcessedQuestion= data_helpers.load_data_and_labels_evaluation(FLAGS.Questions)
Q= program.load_Question_Embeddings(ProcessedQuestion,model)
np.save(FLAGS.QuestionEmbeddings, Q)
	
