# scripts/setup.sh
#!/bin/bash

echo "🚀 Configurando Sistema ITBIS DGII..."

# Crear directorios necesarios
mkdir -p logs
mkdir -p data

# Configurar permisos para SQL Server
if [[ "$OSTYPE" == "linux-gnu"* ]]; then
    echo "📁 Configurando permisos para SQL Server en Linux..."
    sudo chown -R 10001:0 data/
fi

# Verificar instalación de Docker
if ! command -v docker &> /dev/null; then
    echo "❌ Docker no está instalado. Por favor instala Docker Desktop."
    exit 1
fi

if ! command -v docker-compose &> /dev/null; then
    echo "❌ Docker Compose no está instalado."
    exit 1
fi

echo "✅ Docker y Docker Compose están instalados."

# Crear archivo .env si no existe
if [ ! -f .env ]; then
    echo "📝 Creando archivo .env..."
    cat > .env << EOL
# Configuración de Base de Datos
SA_PASSWORD=YourStrongPassword123!
DB_NAME=DbItbisDgii
IDENTITY_DB_NAME=IdentityItbisDgii

# Configuración JWT
JWT_KEY=1A292DE6-0E73-4D6D-9F32-8DB6A83E57BE
JWT_ISSUER=ITBISAppIssuer
JWT_AUDIENCE=ITBISAppAudience
JWT_DURATION=30

# Puertos
API_PORT=5265
FRONTEND_PORT=3000
SQL_PORT=1433
EOL
    echo "✅ Archivo .env creado."
fi

# Crear archivo .env para frontend si no existe
if [ ! -f itbis-dgii-frontend/.env ]; then
    echo "📝 Creando archivo .env para frontend..."
    cat > itbis-dgii-frontend/.env << EOL
VITE_API_URL=http://localhost:5265/api
VITE_APP_NAME=ITBIS DGII Sistema
VITE_APP_VERSION=1.0.0
EOL
    echo "✅ Archivo .env para frontend creado."
fi

echo "✅ Configuración completada!"
echo ""
echo "📋 Comandos disponibles:"
echo "  🚀 Iniciar todos los servicios:    docker-compose up -d"
echo "  👁️  Ver logs:                      docker-compose logs -f"
echo "  🛑 Detener servicios:             docker-compose down"
echo "  🔄 Reconstruir:                   docker-compose up --build"
echo ""
echo "🌐 URLs una vez iniciado:"
echo "  Frontend: http://localhost:3000"
echo "  API:      http://localhost:5265"
echo "  Swagger:  http://localhost:5265/swagger"