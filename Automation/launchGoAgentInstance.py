import awsUtils
import boto3

ec2 = boto3.resource('ec2')
instance = awsUtils.launchAutoregisterGoAgentWindows(ec2, 720)

text_file = open("Artifacts\EC2GoAgentInstanceID.txt", "w")
text_file.write(instance.id)
text_file.close()
