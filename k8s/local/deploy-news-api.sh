kubectl apply -f deploy-news-api.yml
kubectl port-forward service/techblog-newsapi-api 7070:80

