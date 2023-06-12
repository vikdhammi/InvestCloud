# InvestCloud
InvestCloud .Net Technical Interview Question
A service exists that provides numerical data from a pair of two-dimensional datasets A and B. The
contents and dimensions of A and B can be interpreted as two 2D square matrices, which when
multiplied together produce a third matrix that is the desired result of this coding test.
Write a program that retrieves the datasets A & B, multiplies their matrix representations (A X B), and
submits the result back to the service.
1. The service API description at https://recruitment-test.investcloud.com/.
2. Initialize the dataset size to 1000 x 1000 elements. Doesn't count towards total runtime.
3. The result matrix must be formatted as a concatenated string of the matrix' contents (left-toright, top-to-bottom), hashed using the md5 algorithm. Submit the md5 hash to validate your
result and receive a passphrase from the service indicating success or failure.
4. Total runtime should be as fast as possible, given the size of the datasets, the nature of the
service API, and the mathematical operation requested (cross product of 2 matrices)
5. For your reference, we have an implementation that completes with a runtime of data
retrieval and computation in around 30 seconds, understood this depends on various factors.
Reference for Matrix Multiplication (A X B):
Submission Instructions
Do not email .zip files because they will be rejected.
Please submit a link to a downloadable source code for your solution. It can be Google Drive, One Drive
or Dropbox. Or you can upload it to GitHub or another Git repository of your choosing.
