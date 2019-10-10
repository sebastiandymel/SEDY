# Docker-compose deployment

0. Create `configuration.json` from `configuration.docker.json` 
1. Execute `docker-compose build`
2. Execute `docker-compose up`
3. Open in the browser:
- `http://localhost:5400/swagger/index.html`
- `http://localhost:8500`


# K8s deployment with minikube

0. Execute `minikube start`
1. Execute `minikube docker-env | Invoke-Expression` to make docker user the k8s docker. **Works only in scope of this console!**
3. Execute `docker-compose -p 'training_synch_com' build` to build the image.
3. Run `kubectl apply -f .\deployment.yml`
4. Run `kubectl get pods` to make sure pods got the image.
5. After updating images run `kubectl delete -f .\deployment.yml` to remove previous deployment and make k8s take fresh images.
6. Access minikube
	1. Forward the port of the serviceapi `kubectl port-forward svc/serviceapi 5400:5400`
	2. Get the minikube ip address using `minikube ip` and replace the `Client` `_hostUrl` propert to it. Also change the port.



# Troubleshooting

- check if pods can see the docker image `kubectl get pods`
- check if images are present in docker using `docker images`
- remove all images `docker rmi $(docker images -aq)`