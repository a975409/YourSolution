ASP.NET Core開發用的範例專案

箭頭方向是指依賴方向

用戶請求 → 表現層 → 應用層 → 領域層
             ↑         ↑
             |         |
          基礎設施層 ← 基礎設施層

外部串接服務 → 基礎設施層

範例專案結構說明：
1. 領域層：src/Core/YourSolution.Domain
2. 應用層：src/Core/YourSolution.Application
3. 基礎設施層：src/Infrastructure/YourSolution.Infrastructure
   外部串接服務：src/Infrastructure/YourSolution.ExternalServices

4. 表現層：
   - Web：src/Presentation/YourSolution.Web
   - WepApi：src/Presentation/YourSolution.WebApi
