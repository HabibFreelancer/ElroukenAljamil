#!/bin/bash
# =============================================================
# Script d'initialisation HashiCorp Vault
# Usage : bash vault/init-vault.sh
# Prérequis : VAULT_ADDR et VAULT_TOKEN exportés, vault CLI installé
# =============================================================

set -euo pipefail

VAULT_ADDR="${VAULT_ADDR:-http://localhost:8200}"
export VAULT_ADDR

echo "=== Connexion à Vault : $VAULT_ADDR ==="
vault status

# Activer le moteur KV v2 sur le chemin 'secret'
vault secrets enable -path=secret kv-v2 2>/dev/null || echo "KV v2 déjà activé"

# =============================================================
# SECRETS PARTAGÉS (tous les microservices)
# =============================================================
echo "--- Écriture des secrets partagés ---"
vault kv put secret/elrouken/shared \
  JWT_SECRET="${JWT_SECRET:?JWT_SECRET requis}" \
  RABBITMQ_USER="${RABBITMQ_USER:?RABBITMQ_USER requis}" \
  RABBITMQ_PASSWORD="${RABBITMQ_PASSWORD:?RABBITMQ_PASSWORD requis}" \
  DB_USER="${DB_USER:?DB_USER requis}" \
  DB_PASSWORD="${DB_PASSWORD:?DB_PASSWORD requis}"

# =============================================================
# SECRETS PAR SERVICE
# =============================================================
echo "--- Écriture des secrets Media ---"
vault kv put secret/elrouken/media \
  MINIO_ACCESS_KEY="${MINIO_ACCESS_KEY:?MINIO_ACCESS_KEY requis}" \
  MINIO_SECRET_KEY="${MINIO_SECRET_KEY:?MINIO_SECRET_KEY requis}"

echo "--- Écriture des secrets Notification ---"
vault kv put secret/elrouken/notification \
  BREVO_SMTP_USER="${BREVO_SMTP_USER:?BREVO_SMTP_USER requis}" \
  BREVO_SMTP_PASSWORD="${BREVO_SMTP_PASSWORD:?BREVO_SMTP_PASSWORD requis}" \
  BREVO_API_KEY="${BREVO_API_KEY:?BREVO_API_KEY requis}"

echo "--- Écriture des secrets Listings ---"
vault kv put secret/elrouken/listings \
  HUGGINGFACE_TOKEN="${HUGGINGFACE_TOKEN:-}"

echo "--- Écriture des secrets Search ---"
vault kv put secret/elrouken/search \
  ELASTIC_USER="${ELASTIC_USER:-}" \
  ELASTIC_PASSWORD="${ELASTIC_PASSWORD:-}"

# =============================================================
# POLITIQUE D'ACCÈS (lecture seule pour les microservices)
# =============================================================
echo "--- Création de la politique microservices ---"
vault policy write elrouken-services - <<EOF
path "secret/data/elrouken/shared" {
  capabilities = ["read"]
}
path "secret/data/elrouken/listings" {
  capabilities = ["read"]
}
path "secret/data/elrouken/identity" {
  capabilities = ["read"]
}
path "secret/data/elrouken/messaging" {
  capabilities = ["read"]
}
path "secret/data/elrouken/media" {
  capabilities = ["read"]
}
path "secret/data/elrouken/search" {
  capabilities = ["read"]
}
path "secret/data/elrouken/notification" {
  capabilities = ["read"]
}
path "secret/data/elrouken/apigateway" {
  capabilities = ["read"]
}
EOF

# Créer un token de service avec la politique (TTL 720h = 30 jours)
echo "--- Création du token de service ---"
vault token create \
  -policy=elrouken-services \
  -ttl=720h \
  -display-name="elrouken-microservices" \
  -format=json | jq -r '.auth.client_token' > vault/service-token.txt

echo ""
echo "=== Initialisation terminée ==="
echo "Token de service sauvegardé dans vault/service-token.txt"
echo "Exporter : export VAULT_TOKEN=\$(cat vault/service-token.txt)"
