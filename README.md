# EcommerceMicroserviceProject

Dans le cadre d'un projet de conception d'un projet de commerce électronique ECommerce, j'ai opter pour une architecture de microservices avec une passerelle API comprend des services indépendants pour gérer des fonctions commerciales spécifiques telles que les produits, les commandes et les comptes d'utilisateurs. La passerelle API sert de point d'entrée unique pour gérer les demandes des clients, les acheminer vers les microservices appropriés et appliquer les politiques de sécurité. La limitation du débit contrôle le nombre de requêtes qu'un client peut effectuer afin d'éviter les abus et de garantir une utilisation équitable. La mise en cache stocke les données fréquemment consultées afin d'améliorer les performances et de réduire la charge sur les services. Les stratégies de réessai garantissent la fiabilité en réessayant automatiquement les requêtes qui échouent en raison d'erreurs transitoires. Cette configuration améliore l'évolutivité, les performances et la résilience des plateformes d'achat en ligne.
En optant pour une architecture en oignon pour facilité la maintenabilté de l'application.


<img width="379" alt="image" src="https://github.com/user-attachments/assets/6da850fe-8f01-45aa-b6bb-6c34d28eb1e8" />



Tests unitaire (en cours)

