import awsUtils
import boto3

#iam = boto3.resource('iam')
#current_user = iam.CurrentUser()

#print(current_user.user_name)

ec2 = boto3.resource('ec2')
instances = ec2.instances.all()
awsUtils.stopExpiredInstances(instances)


