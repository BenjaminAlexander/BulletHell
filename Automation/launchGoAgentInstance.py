import awsUtils
import boto3

ec2 = boto3.resource('ec2')
instance = awsUtils.launchAutoregisterGoAgentWindows(ec2, 720, "4f6482d7-9a7c-4ced-9b4d-694ee9f345c2")

text_file = open("Artifacts\EC2GoAgentInstanceID.txt", "w")
text_file.write(instance.id)
text_file.close()
