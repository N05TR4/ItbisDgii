@echo off
REM scripts/setup.bat
echo 🚀 Configurando Sistema ITBIS DGII...

REM Crear directorios necesarios
if not exist "logs" mkdir logs
if not exist "data" mkdir data

REM Verificar instalación de Docker
docker --version >nul 2>&1
if errorlevel 1 (
    echo ❌ Docker no está instalado. Por favor instala Docker Desktop.
    pause
    exit /b 1
)

docker-compose --version >nul 2>&1
if errorlevel 1 (
    echo ❌ Docker Compose no está instalado.
    pause
    exit /b 1
)

echo ✅ Docker y Docker Compose están instalados.

REM Crear archivo .env si no existe
if not exist ".env" (
    echo 📝 Creando archivo .env...
    (
        echo # Configuración de Base de Datos
        echo SA_PASSWORD=YourStrongPassword123!
        echo DB_NAME=DbItbisDgii
        echo IDENTITY_DB_NAME=IdentityItbisDgii
        echo.
        echo # Configuración JWT
        echo JWT_KEY=1A292DE6-0E73-4D6D-9F32-8DB6A83E57BE
        echo JWT_ISSUER=ITBISAppIssuer
        echo JWT_AUDIENCE=ITBISAppAudience
        echo JWT_DURATION=30
        echo.
        echo # Puertos
        echo API_PORT=5265
        echo FRONTEND_PORT=3000
        echo SQL_PORT=1433
    ) > .env
    echo ✅ Archivo .env creado.
)

REM Crear archivo .env para frontend si no existe
if not exist "itbis-dgii-frontend\.env" (
    echo 📝 Creando archivo .env para frontend...
    (
        echo VITE_API_URL=http://localhost:5265/api
        echo VITE_APP_NAME=ITBIS DGII Sistema
        echo VITE_APP_VERSION=1.0.0
    ) > itbis-dgii-frontend\.env
    echo ✅ Archivo .env para frontend creado.
)

echo ✅ Configuración completada!
echo.
echo 📋 Comandos disponibles:
echo   🚀 Iniciar todos los servicios:    docker-compose up -d
echo   👁️  Ver logs:                      docker-compose logs -f
echo   🛑 Detener servicios:             docker-compose down
echo   🔄 Reconstruir:                   docker-compose up --build
echo.
echo 🌐 URLs una vez iniciado:
echo   Frontend: http://localhost:3000
echo   API:      http://localhost:5265
echo   Swagger:  http://localhost:5265/swagger
echo.
pause