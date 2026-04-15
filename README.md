Performance Control Layer (Example Project)
📌 Overview

This project demonstrates a Performance Control Layer architecture designed to measure, analyze, and trace system behavior in integration-driven environments.

Instead of focusing on building a production system, this project focuses on proving architectural capability in:

Observability
Dependency performance tracking
Latency and timeout analysis
Error classification
End-to-end request tracing

Although demonstrated in a healthcare context (HBYS + LAB + PACS + SMS), the architecture applies to any modern distributed system.

🎯 Problem Statement

In large-scale systems, performance issues are often misunderstood.

Users say:

“The system is slow.”

But in reality:

External services may be delayed
Some dependencies may timeout
Others may return intermittent errors
The core system may not be the problem at all

Without proper observability, root cause analysis becomes guesswork.

🧠 Solution Approach

This project introduces a Performance Control Layer that enables:

Measuring inbound request performance
Tracking outbound dependency behavior
Separating latency, timeout, and error scenarios
Correlating request chains using CorrelationId

This transforms performance analysis from assumption-based to data-driven.

🏗 Architecture

The solution consists of the following components:

Core System
HBYS API (.NET 9 Web API)
External Dependency Simulations
LAB Mock (Minimal API) → Simulates latency
PACS Mock (Minimal API) → Simulates timeouts
SMS Mock (Minimal API) → Simulates HTTP 500 errors
🔍 Observability Components
🔹 CorrelationId Middleware
Generates and propagates X-Correlation-Id
Enables full request chain tracing across systems
🔹 RequestTiming Middleware (Inbound Metrics)

Tracks:

Path
HTTP Method
Status Code
Duration
CorrelationId
🔹 DependencyTrackingHandler (Outbound Metrics)

Tracks:

Target system (LAB / PACS / SMS)
Target endpoint
Duration
Status code (null in timeout cases)
Success / failure state
Timeout flag
CorrelationId
🔹 InMemoryMetricStore
Temporary metric storage (in-memory)
Used for real-time inspection during development
🌐 Port Configuration
Service	URL
HBYS API	http://localhost:2000

LAB Mock	http://localhost:2001

PACS Mock	http://localhost:2002

SMS Mock	http://localhost:2003
⚙️ Configuration
appsettings.json
{
  "Dependencies": {
    "LAB": "http://localhost:2001/",
    "PACS": "http://localhost:2002/",
    "SMS": "http://localhost:2003/"
  }
}
🧪 Running the Project
Step 1 — Start Mock Services
LAB → http://localhost:2001/health
PACS → http://localhost:2002/health
SMS → http://localhost:2003/health
Step 2 — Start HBYS API
Swagger UI: http://localhost:2000/swagger
🧪 Test Endpoints
🔹 Health Check
GET /api/ping
🔹 Smoke Tests (Generate Traffic)
LAB
POST /api/smoke/lab
PACS
GET /api/smoke/pacs/{id}
SMS
POST /api/smoke/sms
🔹 Metrics
Dependency Metrics
GET /metrics/debug/deps?take=50
Request Metrics
GET /metrics/debug/requests?take=50
📊 What This Project Demonstrates
Clear separation of inbound vs outbound performance
Detection of latency vs timeout vs error scenarios
Dependency-level observability
Correlation-based tracing
Advanced HttpClient usage
Middleware-driven architecture
🧠 Key Insights
Latency, timeout, and errors are different failure types
Without correlation, tracing is incomplete
Performance must be measured at dependency level
Observability must be designed into the system
🌍 Applicability

This architecture is not limited to healthcare systems.

It applies to:

FinTech platforms
E-commerce systems
Logistics and tracking systems
Microservices architectures
Enterprise integration platforms
🚀 Future Improvements
Persistent storage (database integration)
Time-based aggregation (e.g., last 5-minute timeout rates)
SLA monitoring dashboards
Alerting mechanisms
Real-time observability UI
Centralized telemetry system
🏁 Conclusion

This project demonstrates that:

Performance is not about speed.
It is about visibility.

By separating inbound and outbound metrics and introducing correlation-based tracing, this architecture enables true root cause analysis in integration-driven systems.

📎 Keywords

#SystemDesign #Observability #DistributedSystems #BackendEngineering #PerformanceEngineering #SoftwareArchitecture
