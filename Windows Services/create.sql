#Filename:		create.sql
#Project:		Window and Mobile Programming Assignment #6 And Relational Database Assignment 4
#By:			Zheng Hua/Shaohua Mao
#Date:			2015.11.27
#Description:	This file contains the database creation sql statement.
#				This database will contains 4 entries of this program.
#				1.Questions, 2.Choices, 3.Users, 4.userScore

#create a database for use and use this database
CREATE database questions;
USE questions;

#create a table Questions to represent all questions
CREATE TABLE Questions
(
	questionID INTEGER PRIMARY KEY auto_increment,
    questionText VARCHAR(255) NOT NULL,
    correctChoiceID INTEGER NOT NULL
);

#create a table Choices to represent all choices for the questions
CREATE TABLE Choices
(
	choiceID INTEGER PRIMARY KEY auto_increment,
    choiceText VARCHAR(255) NOT NULL,
    questionID INTEGER
);

#add constraint to questionID in table Choices
ALTER TABLE Choices ADD FOREIGN KEY (questionID) REFERENCES Questions(questionID);

#create a table Users to represent all users who have played this game
CREATE TABLE Users
(
	userID INTEGER PRIMARY KEY auto_increment,
    userName VARCHAR(255) NOT NULL,
    alive bool NOT NULL
); 


#create a table userScore to represent user score for a certain question
CREATE TABLE userScore
(
	userID INTEGER,
    questionID INTEGER,
    score INTEGER NOT NULL
); 

#add constraint to userID and questionID in table userScore
ALTER TABLE userScore ADD FOREIGN KEY (userID) REFERENCES Users(userID);
ALTER TABLE userScore ADD FOREIGN KEY (questionID) REFERENCES Questions(questionID);

#Create a user called "admin" with access from any host, and with full privileges
GRANT ALL PRIVILEGES ON questions.* to 'admin'@'%' IDENTIFIED BY '1234';

#put some test informaiton into table questions
#question 1
INSERT INTO questions (questionText, correctChoiceID) 
VALUES ("If it took eight men ten hours to build a wall,how long would it take four men to build it?", "4");

#question 2
INSERT INTO questions (questionText, correctChoiceID) 
VALUES ("If you divide 30 by half and add ten. What do you get?", "8");

#question 3
INSERT INTO questions (questionText, correctChoiceID) 
VALUES ("If you had only one match, and entered a dark room containing an oil lamp, some newspaper, and some kindling wood, which would you light first? ", "12");

#question 4
INSERT INTO questions (questionText, correctChoiceID) 
VALUES ("Some months have 31 days, others have 30 days. How many have 28 days?", "16");

#question 5
INSERT INTO questions (questionText, correctChoiceID) 
VALUES ("Uncle Joe's farm had a terrible storm. All but seven sheep were killed. How many sheep are still alive?", "20");

#question 6
INSERT INTO questions (questionText, correctChoiceID) 
VALUES ("A rooster laid an egg on top of the barn roof. Which way did it roll?", "24");

#question 7
INSERT INTO questions (questionText, correctChoiceID) 
VALUES ("Which is heavier a ton of gold or a ton of silver?", "28");

#question 8
INSERT INTO questions (questionText, correctChoiceID) 
VALUES ("A doctor gives you 5 pills and tells you to take one pill every half an hour. How many hours passed until all the pills are taken?", "32");

#question 9
INSERT INTO questions (questionText, correctChoiceID) 
VALUES ("Who's bigger: Mr. Bigger, Mrs. Bigger or their baby? ", "36");

INSERT INTO questions (questionText, correctChoiceID) 
VALUES ("If there are 6 apples and you take away 4, how many do you have?", "37");



#put some test informaiton into table Choices
INSERT INTO Choices (choiceText, questionID) VALUES ("They can't build it with only 4 men", "1");
INSERT INTO Choices (choiceText, questionID) VALUES ("Twenty hours", "1");
INSERT INTO Choices (choiceText, questionID) VALUES ("Eighty hours", "1");
INSERT INTO Choices (choiceText, questionID) VALUES ("No time at all it is already built", "1");

INSERT INTO Choices (choiceText, questionID) VALUES ("50", "2");
INSERT INTO Choices (choiceText, questionID) VALUES ("3", "2");
INSERT INTO Choices (choiceText, questionID) VALUES ("25", "2");
INSERT INTO Choices (choiceText, questionID) VALUES ("70", "2");

INSERT INTO Choices (choiceText, questionID) VALUES ("Newspaper", "3");
INSERT INTO Choices (choiceText, questionID) VALUES ("Kindling wood", "3");
INSERT INTO Choices (choiceText, questionID) VALUES ("Oil lamp", "3");
INSERT INTO Choices (choiceText, questionID) VALUES ("The match", "3");

INSERT INTO Choices (choiceText, questionID) VALUES ("2", "4");
INSERT INTO Choices (choiceText, questionID) VALUES ("0", "4");
INSERT INTO Choices (choiceText, questionID) VALUES ("1", "4");
INSERT INTO Choices (choiceText, questionID) VALUES ("12", "4");

INSERT INTO Choices (choiceText, questionID) VALUES ("One hundred", "5");
INSERT INTO Choices (choiceText, questionID) VALUES ("All", "5");
INSERT INTO Choices (choiceText, questionID) VALUES ("Zero", "5");
INSERT INTO Choices (choiceText, questionID) VALUES ("even", "5");

INSERT INTO Choices (choiceText, questionID) VALUES ("East", "6");
INSERT INTO Choices (choiceText, questionID) VALUES ("Lower roof", "6");
INSERT INTO Choices (choiceText, questionID) VALUES ("Higher roof", "6");
INSERT INTO Choices (choiceText, questionID) VALUES ("It didn't roll", "6");

INSERT INTO Choices (choiceText, questionID) VALUES ("Gold", "7");
INSERT INTO Choices (choiceText, questionID) VALUES ("Silver", "7");
INSERT INTO Choices (choiceText, questionID) VALUES ("The same", "7");
INSERT INTO Choices (choiceText, questionID) VALUES ("None", "7");

INSERT INTO Choices (choiceText, questionID) VALUES ("0", "8");
INSERT INTO Choices (choiceText, questionID) VALUES ("3", "8");
INSERT INTO Choices (choiceText, questionID) VALUES ("2.5", "8");
INSERT INTO Choices (choiceText, questionID) VALUES ("2", "8");

INSERT INTO Choices (choiceText, questionID) VALUES ("None", "9");
INSERT INTO Choices (choiceText, questionID) VALUES ("Mrs. Bigger", "9");
INSERT INTO Choices (choiceText, questionID) VALUES ("Mr. Bigger", "9");
INSERT INTO Choices (choiceText, questionID) VALUES ("The baby, since he is a little Bigger.", "9");

INSERT INTO Choices (choiceText, questionID) VALUES ("4", "10");
INSERT INTO Choices (choiceText, questionID) VALUES ("2", "10");
INSERT INTO Choices (choiceText, questionID) VALUES ("6", "10");
INSERT INTO Choices (choiceText, questionID) VALUES ("0", "10");