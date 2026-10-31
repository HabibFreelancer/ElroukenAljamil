storage "file" {
  path = "/vault/data"
}

listener "tcp" {
  address     = "0.0.0.0:8200"
  tls_disable = 1
}

# En production, activer TLS :
# listener "tcp" {
#   address       = "0.0.0.0:8200"
#   tls_cert_file = "/vault/tls/vault.crt"
#   tls_key_file  = "/vault/tls/vault.key"
# }

ui            = true
api_addr      = "http://0.0.0.0:8200"
cluster_addr  = "http://0.0.0.0:8201"
