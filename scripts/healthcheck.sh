#!/bin/bash
# scripts/healthcheck.sh

echo "🏥 Verificando estado de los servicios..."

# Función para verificar si un servicio está corriendo
check_service() {
    local service_name=$1
    local url=$2
    local expected_status=${3:-200}
    
    echo -n "Verificando $service_name... "
    
    if curl -s -o /dev/null -w "%{http_code}" "$url" | grep -q "$expected_status"; then
        echo "✅ OK"
        return 0
    else
        echo "❌ FALLO"
        return 1
    fi
}

# Función para verificar conexión a base de datos
check_database() {
    echo -n "Verificando Base de Datos... "
    
    if docker-compose exec -T sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "YourStrongPassword123!" -Q "SELECT 1" &>/dev/null; then
        echo "✅ OK"
        return 0
    else
        echo "❌ FALLO"
        return 1
    fi
}

# Esperar a que los servicios estén disponibles
echo "⏳ Esperando a que los servicios estén listos..."
sleep 10

# Variables
API_URL="http://localhost:5265/swagger"
FRONTEND_URL="http://localhost:3000"

# Verificaciones
services_ok=0

# Verificar base de datos
if check_database; then
    ((services_ok++))
fi

# Verificar API
if check_service "API (Swagger)" "$API_URL"; then
    ((services_ok++))
fi

# Verificar Frontend
if check_service "Frontend" "$FRONTEND_URL"; then
    ((services_ok++))
fi

echo ""
echo "📊 Resumen:"
echo "Services OK: $services_ok/3"

if [ $services_ok -eq 3 ]; then
    echo "🎉 ¡Todos los servicios están funcionando correctamente!"
    echo ""
    echo "🌐 URLs disponibles:"
    echo "  Frontend: http://localhost:3000"
    echo "  API: http://localhost:5265"
    echo "  Swagger: http://localhost:5265/swagger"
    echo ""
    echo "👤 Usuarios de prueba:"
    echo "  Admin: userAdmin@mail.com / Admin123!"
    echo "  Básico: userBasic@mail.com / Basic123!"
    exit 0
else
    echo "⚠️  Algunos servicios no están funcionando correctamente."
    echo "💡 Ejecuta 'docker-compose logs' para ver los detalles."
    exit 1
fi