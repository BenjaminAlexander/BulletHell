import awsUtils
import boto3

ec2 = boto3.resource('ec2')
instance = awsUtils.launchAutoregisterGoAgentWindows(ec2, 720)
