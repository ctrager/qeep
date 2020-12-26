# scp this to home folder, for example /home/corey, or /root
# this is the script that executes ON the remote server

cd qeep

git pull
# so that we can look on the server and see what we are deploying
git log -3 > git_log.txt
dotnet publish -o ../qpublish_next
cp qeep_config_active.txt ../qqpublish_next
cp git_log.txt ../qpublish_next
touch ../qpublish_next/$( date '+%Y-%m-%d_%H-%M-%S' )

cd ..

# swap folders

rm -rf qpublish_prev
mv qpublish qpublish_prev
mv qpublish_next qpublish
systemctl restart qeep
