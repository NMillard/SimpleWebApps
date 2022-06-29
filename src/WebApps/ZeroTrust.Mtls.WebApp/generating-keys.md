
1. Generate an rsa key pair
`openssl genrsa -out root-ca.key 4096`  

2. Create a private certificate authority (CA)
`openssl req -new -x509 -days 3650 -key root-ca.key -out root-ca.pem`  

3. Create client certificate private key and certifcate signing request (CSR)
`openssl genrsa -out client.key 2048`  
`openssl req -new -key client.key -out client.csr`  

4. Sign the newly created client certficate using the private CA  
`openssl x509 -req -in client.csr -CA root-ca.pem -CAkey root-ca.key -set_serial 01 -out client.pem -days 3650 -sha256`

5. Copy the private CA pem file to a keystore/truststore.


https://crypto.stackexchange.com/questions/43697/what-are-the-differences-between-pem-csr-key-crt-and-other-such-file-exte
https://aws.amazon.com/blogs/compute/introducing-mutual-tls-authentication-for-amazon-api-gateway/