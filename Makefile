# Makefile

.PHONY: help setup up down build logs clean test migrate

# Variables
COMPOSE_FILE = docker-compose.yml
COMPOSE_DEV_FILE = docker-compose.dev.yml

# Ayuda
help:
	@echo "🚀 Sistema ITBIS DGII - Comandos disponibles:"
	@echo ""
	@echo "📦 Configuración:"
	@echo "  setup      - Configuración inicial del proyecto"
	@echo "  build      - Construir imágenes Docker"
	@echo ""
	@echo "🚀 Ejecución:"
	@echo "  up         - Iniciar todos los servicios"
	@echo "  up-dev     - Iniciar solo base de datos (desarrollo)"
	@echo "  down       - Detener todos los servicios"
	@echo "  restart    - Reiniciar servicios"
	@echo ""
	@echo "🔍 Monitoreo:"
	@echo "  logs       - Ver logs de todos los servicios"
	@echo "  logs-api   - Ver logs del API"
	@echo "  logs-fe    - Ver logs del frontend"
	@echo "  logs-db    - Ver logs de la base de datos"
	@echo ""
	@echo "🧪 Desarrollo:"
	@echo "  test       - Ejecutar pruebas"
	@echo "  migrate    - Ejecutar migraciones"
	@echo "  seed       - Sembrar datos de prueba"
	@echo ""
	@echo "🧹 Limpieza:"
	@echo "  clean      - Limpiar contenedores y volúmenes"
	@echo "  reset      - Reset completo del proyecto"

# Configuración inicial
setup:
	@echo "🔧 Configurando proyecto..."
	@chmod +x scripts/setup.sh
	@./scripts/setup.sh

# Construir imágenes
build:
	@echo "🏗️  Construyendo imágenes..."
	@docker-compose -f $(COMPOSE_FILE) build

# Iniciar servicios completos
up:
	@echo "🚀 Iniciando todos los servicios..."
	@docker-compose -f $(COMPOSE_FILE) up -d
	@echo "✅ Servicios iniciados!"
	@echo "🌐 Frontend: http://localhost:3000"
	@echo "🚀 API: http://localhost:5265"
	@echo "📊 Swagger: http://localhost:5265/swagger"

# Iniciar solo base de datos para desarrollo
up-dev:
	@echo "🛠️  Iniciando servicios de desarrollo..."
	@docker-compose -f $(COMPOSE_DEV_FILE) up -d
	@echo "✅ Base de datos iniciada!"
	@echo "🗄️  SQL Server: localhost:1433"

# Detener servicios
down:
	@echo "🛑 Deteniendo servicios..."
	@docker-compose -f $(COMPOSE_FILE) down
	@docker-compose -f $(COMPOSE_DEV_FILE) down
	@echo "✅ Servicios detenidos!"

# Reiniciar servicios
restart: down up

# Ver logs
logs:
	@docker-compose -f $(COMPOSE_FILE) logs -f

logs-api:
	@docker-compose -f $(COMPOSE_FILE) logs -f api

logs-fe:
	@docker-compose -f $(COMPOSE_FILE) logs -f frontend

logs-db:
	@docker-compose -f $(COMPOSE_FILE) logs -f sqlserver

# Ejecutar pruebas
test:
	@echo "🧪 Ejecutando pruebas..."
	@docker-compose exec api dotnet test

# Ejecutar migraciones
migrate:
	@echo "🗄️  Ejecutando migraciones..."
	@docker-compose exec api dotnet ef database update --project ItbisDgii.Infrastructure

# Sembrar datos
seed:
	@echo "🌱 Sembrando datos de prueba..."
	@docker-compose exec api dotnet run --project ItbisDgii.API -- --seed

# Limpiar contenedores y volúmenes
clean:
	@echo "🧹 Limpiando contenedores y volúmenes..."
	@docker-compose -f $(COMPOSE_FILE) down -v --remove-orphans
	@docker-compose -f $(COMPOSE_DEV_FILE) down -v --remove-orphans
	@docker system prune -f
	@echo "✅ Limpieza completada!"

# Reset completo
reset: clean
	@echo "🔄 Reset completo del proyecto..."
	@docker-compose -f $(COMPOSE_FILE) down -v --remove-orphans --rmi all
	@docker-compose -f $(COMPOSE_DEV_FILE) down -v --remove-orphans --rmi all
	@echo "✅ Reset completado!"

# Comandos de desarrollo local
dev-api:
	@echo "🚀 Iniciando API en modo desarrollo..."
	@cd ItbisDgii.API && dotnet run

dev-frontend:
	@echo "🌐 Iniciando Frontend en modo desarrollo..."
	@cd itbis-dgii-frontend && npm run dev

# Instalar dependencias frontend
install-fe:
	@echo "📦 Instalando dependencias del frontend..."
	@cd itbis-dgii-frontend && npm install

# Build frontend
build-fe:
	@echo "🏗️  Construyendo frontend..."
	@cd itbis-dgii-frontend && npm run build

# Comandos de utilidad
shell-api:
	@docker-compose exec api bash

shell-db:
	@docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrongPassword123!

backup-db:
	@echo "💾 Creando backup de base de datos..."
	@docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrongPassword123! -Q "BACKUP DATABASE DbItbisDgii TO DISK = '/var/opt/mssql/backup/DbItbisDgii.bak'"

# Status de servicios
status:
	@echo "📊 Estado de los servicios:"
	@docker-compose -f $(COMPOSE_FILE) ps