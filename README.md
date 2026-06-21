# DevSpace 💻

**DevSpace** es un foro de discusión web diseñado para estudiantes de programación y desarrolladores junior. Permite a los usuarios plantear dudas técnicas, compartir fragmentos de código con formato y marcar las respuestas que resuelven con éxito sus problemas, emulando la dinámica de StackOverflow.

Proyecto final del curso **SC-601 Programación Avanzada**.

---

## 👤 Integrantes

| Nombre | GitHub User |
|---|---|
| Carlos Eduardo Ramírez Wong | CarlosRW |
| Christian Soto Robles | Brightax06 |
| Isaac Daniel Cardenas Araya | Mizifuz |
| Monica Vargas Abarca | morava14 | 

---

## 📌 Estado del proyecto

> Este README se actualiza en cada avance del curso. Estado actual: **Avance 1**.

- [x] Arquitectura en capas (Data / Models / Core / MVC)
- [x] Base de datos y script de creación
- [x] Login y registro de usuario
- [ ] Landing page
- [ ] Pantalla de mantenimiento (crear/editar preguntas y respuestas)
- [ ] Principios SOLID documentados en código
- [ ] Design Patterns documentados en código

---

**Flujo de una petición:** `Vista (MVC)` → `Controller` → `Business (Core)` → `Repository (Data)` → `SQL Server`

### Stack
- ASP.NET MVC 5 (.NET Framework 4.8.1)
- ADO.NET puro (`SqlConnection` / `SqlCommand`) — sin ORM
- SQL Server
- Bootstrap 5 + jQuery
- Forms Authentication

---

## 🗄️ Base de datos

El script de creación está en [`DataBase/Script_BD_DevSpace.sql`](./DataBase/Script_BD_DevSpace.sql). Crea automáticamente la base `DevSpaceDB` (si no existe) y sus tres tablas principales:

| Tabla        | Descripción                                              |
|--------------|-----------------------------------------------------------|
| `Usuarios`   | Cuentas registradas (correo único, password hasheado)     |
| `Preguntas`  | Dudas técnicas planteadas por los usuarios                 |
| `Respuestas` | Respuestas asociadas a una pregunta                        |

### Cómo levantar la base
1. Abrí SQL Server Management Studio.
2. Conectate a tu instancia local (ej. `localhost\SQLEXPRESS`).
3. Ejecutá el script completo `Script_BD_DevSpace.sql`.

---

## ⚙️ Configuración local

1. Cloná el repositorio.
2. Abrí `ForoProgramacion.sln` en Visual Studio.
3. Restaurá los paquetes NuGet si VS no lo hace automático (`Tools` → `NuGet Package Manager` → `Restore`).
4. Editá la cadena de conexión en `TechForo.MVC/Web.config`:

   ```xml
   <connectionStrings>
     <add name="ForoConnectionString"
          connectionString="Server=TU_INSTANCIA;Database=DevSpaceDB;Integrated Security=True;"
          providerName="System.Data.SqlClient" />
   </connectionStrings>
   ```

   Reemplazá `TU_INSTANCIA` por tu servidor de SQL Server (ej. `localhost\SQLEXPRESS`). Si usás autenticación SQL en lugar de Windows, cambiá `Integrated Security=True;` por `User Id=tu_usuario;Password=tu_password;`.

5. Asegurate de que `TechForo.MVC` esté marcado como **Startup Project** (clic derecho sobre el proyecto → `Set as Startup Project`).
6. Ejecutá con `F5`.

---

## 🔐 Funcionalidades implementadas

### Autenticación
- **Registro** (`/Account/Register`): crea un usuario nuevo, valida que el correo no esté repetido, hashea el password con SHA-256 antes de guardarlo.
- **Login** (`/Account/Login`): valida credenciales contra la BD, usa Forms Authentication para mantener la sesión.
- **Logout** (`/Account/Logout`): cierra sesión y limpia la cookie de autenticación.

Las contraseñas **nunca se guardan en texto plano**.

---

## 📄 Licencia

Proyecto académico — Curso SC-601 Programación Avanzada - Universidad Fidélitas
