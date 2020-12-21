# Invoke the deploy script on the remote server

# make sure keep_active_config.txt is in remote keep folder

scp deploy_to_production.sh root@157.230.222.170:/root/keep
ssh root@157.230.222.170  'chmod +x keep/deploy_to_production.sh'
ssh root@157.230.222.170  keep/deploy_to_production.sh