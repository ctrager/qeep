# Invoke the deploy script on the remote server

# make sure qeep_active_config.txt is in remote qeep folder

scp deploy_to_production.sh root@157.230.222.170:/root/qeep
ssh root@157.230.222.170  'chmod +x qeep/deploy_to_production.sh'
ssh root@157.230.222.170  qeep/deploy_to_production.sh