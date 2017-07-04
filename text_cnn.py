import tensorflow as tf
import numpy as np
import lutorpy as lua


class TextCNN(object):
    """
    A CNN for text classification.
    Uses an embedding layer, followed by a convolutional, max-pooling and softmax layer.
    """
    def __init__(
      self, Q_sequence_length,A_sequence_length, num_classes,
      embedding_size, filter_sizes, num_filters, l2_reg_lambda=0.0):

        # Placeholders for input, output and dropout
        self.input_Q = tf.placeholder(tf.float32, [None, Q_sequence_length,300], name="input_Q")
        self.input_A = tf.placeholder(tf.float32, [None, A_sequence_length,300], name="input_A")
        self.input_y = tf.placeholder(tf.float32, [None, num_classes], name="input_y")
        self.dropout_keep_prob = tf.placeholder(tf.float32, name="dropout_keep_prob")
        self.inputLines= tf.placeholder(tf.float32,[None,None], name="Weights_reduce")

        # Keeping track of l2 regularization loss (optional)
        l2_loss = tf.constant(0.0)
        self.input_Q_expanded = tf.expand_dims(self.input_Q, -1)
        self.input_A_expanded = tf.expand_dims(self.input_A, -1)
        

        # Create a convolution + maxpool layer for each filter size
        pooled_outputs_Q = []
        pooled_outputs_A= []
        for i, filter_size in enumerate(filter_sizes):
            filter_shape = [filter_size, embedding_size, 1, num_filters]
            W 	= tf.Variable(tf.truncated_normal(filter_shape, stddev=0.1), name="W")
            b = tf.Variable(tf.constant(0.1, shape=[num_filters]), name="b")
            conv_Q = tf.nn.conv2d(self.input_Q_expanded,W,strides=[1, 1, 1, 1],padding="VALID",name="conv_Q")
            # Apply nonlinearity
            h_Q = tf.nn.relu(tf.nn.bias_add(conv_Q, b), name="relu")
            # Maxpooling over the outputs
            pooled_Q = tf.nn.max_pool(h_Q,ksize=[1, Q_sequence_length - filter_size + 1, 1, 1],strides=[1, 1, 1, 1],padding='VALID',name="pool_Q")
            pooled_outputs_Q.append(pooled_Q)
		
		    #for A
            filter_shape_A = [filter_size, embedding_size, 1, num_filters]
            W_A = tf.Variable(tf.truncated_normal(filter_shape_A, stddev=0.1), name="W_A")
            b_A = tf.Variable(tf.constant(0.1, shape=[num_filters]), name="b_A")
            conv_A = tf.nn.conv2d(self.input_A_expanded,W_A,strides=[1, 1, 1, 1],padding="VALID",name="conv_A")
            # Apply nonlinearity
            h_A = tf.nn.relu(tf.nn.bias_add(conv_A, b_A), name="relu")
            # Maxpooling over the outputs
            pooled_A = tf.nn.max_pool(h_A,ksize=[1, A_sequence_length - filter_size + 1, 1, 1],strides=[1, 1, 1, 1],padding='VALID',name="pool_A")
            pooled_outputs_A.append(pooled_A)
        
        # Combine all the pooled features
        num_filters_total = num_filters * len(filter_sizes)
        self.h_pool_Q = tf.concat(pooled_outputs_Q, 3)
        self.h_pool_flat_Q = tf.reshape(self.h_pool_Q, [-1, num_filters_total])
        self.h_pool_A = tf.concat(pooled_outputs_A, 3)
        self.h_pool_flat_A = tf.reshape(self.h_pool_A, [-1, num_filters_total])
        #self.h_pool_flat= tf.concat((self.h_pool_flat_Q,self.h_pool_flat_A),-1)
        #self.h_pool_flattend =tf.transpose(tf.matmul(tf.transpose(self.h_pool_flat), self.inputLines))
        #print self.h_pool_flattend.get_shape()
		
        self.fcQ = tf.layers.dense(inputs=self.h_pool_flat_Q, units=2*num_filters_total, activation=tf.nn.relu)
        self.fcA = tf.layers.dense(inputs=self.h_pool_flat_A, units=2*num_filters_total, activation=tf.nn.relu)
        
        # Add dropout
        with tf.name_scope("dropout"):
            self.h_drop = tf.nn.dropout(self.fcQ, self.dropout_keep_prob)

        # Final (unnormalized) scores and predictions
        with tf.name_scope("output"):
            W = tf.get_variable(
                "W",
                shape=[2*num_filters_total, num_classes],
                initializer=tf.contrib.layers.xavier_initializer())
            b = tf.Variable(tf.constant(0.1, shape=[num_classes]), name="b")
            l2_loss += tf.nn.l2_loss(W)
            l2_loss += tf.nn.l2_loss(b)
            self.scores = tf.nn.xw_plus_b(self.h_drop, W, b, name="scores")
            self.predictions = tf.argmax(self.scores, 1, name="predictions")

        # CalculateMean cross-entropy loss
        with tf.name_scope("loss"):
            losses = tf.nn.softmax_cross_entropy_with_logits(logits=self.scores, labels=self.input_y)
            self.loss = tf.reduce_mean(losses) + l2_reg_lambda * l2_loss
			
        # Accuracy
        with tf.name_scope("accuracy"):
            correct_predictions = tf.equal(self.predictions, tf.argmax(self.input_y, 1))
            self.accuracy = tf.reduce_mean(tf.cast(correct_predictions, "float"), name="accuracy")