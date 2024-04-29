kubectl apply -f deploy-notification-api.yml
kubectl port-forward service/techblog-notification-api 7060:80
