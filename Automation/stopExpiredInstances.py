import awsUtils
import boto3

ec2 = boto3.resource('ec2')
instances = ec2.instances.all()
awsUtils.stopExpiredInstances(instances)
