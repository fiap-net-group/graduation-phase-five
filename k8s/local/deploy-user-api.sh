kubectl apply -f deploy-users-api.yml
kubectl port-forward service/techblog-users-api 7080:80

