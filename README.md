# Financharts-Api-
Build
docker compose up --build in root of project

Endpoints
First you need to fill the Database with info about available assets 
http://localhost:8080/api/v1/Assets
then you can get prices informaiton about specific asset 
http://localhost:8080/api/v1/PriceInfo?symbols=A 
or about specific assets 
http://localhost:8080/api/v1/PriceInfo?symbols=A&symbols=ABC