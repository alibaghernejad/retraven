# Retraven
**RetRaven** is a cloud-native search and recommendation _Launchpad_ designed for the AI era. It combines the orchestration capabilities of **.NET Aspire** with the machine learning power of **Python-based embedding pipelines**, supporting **sparse retrieval**, **dense embeddings**, **hybrid search**, and **RAG (Retrieval-Augmented Generation)** workflows.

> A cross-platform solution combining a Raven for vector embedding and a Retriever for information retrieval operations. The name "Retraven" is a blend of "Retriever" and "Raven", reflecting its dual functionality.

## Overview
Retraven is built using a modern technology stack:
- **Orchestration and information Retrieval:** .NET Aspire (F# and C#), Simple, Secure and efficent.
- **Embedding Engine:** Python
- **Cloud Infrastructure:** .NET Aspire framework for cloud-native applications
- **Data Ingestion Pipeline:** langchain
- **Object Storage: MinIO** – an open-source, high-performance, S3-compatible object store widely regarded as the de facto standard for self-hosted AI and cloud-native storage.
- **LLM and RAG:** Microsoft Semantic 
- **APIs:** ASP .NET MinimalAPIs via C# and F# functional way via Giraffe.
- **Vector Database:** Qdrant – a high-performance, open-source vector search engine supporting dense, sparse, and hybrid retrieval with named vector support, ideal for semantic search and recommendation systems.
- **Service Broker:** RabbitMQ – a robust and widely adopted open-source message broker, used for reliable task queuing, asynchronous processing, and decoupled microservice communication.

## Architecture
The system consists of two main components:
- Raven: Handles data embedding operations
- Retriever: Manages data retrieval processes

This hybrid approach leverages the strengths of multiple languages while maintaining seamless integration through .NET Aspire's cloud-native capabilities.

---

## Features

- **Hybrid Search**: Combines BM25 + dense + late interaction embeddings.
- **Vector Database Integration**: Uses [Qdrant](https://qdrant.tech/) with named vector support.
- **Orchestration**: .NET Aspire in dev/design; Kubernetes for production.
- **External Embedding Pipeline**: Python (LangChain, SentenceTransformers, Celery, RabbitMQ).
- **Document & Image Support**: Embeds and indexes text, PDFs, DOCX, JSON, and image features.
- **RAG Capable**: Integration-ready for Retrieval-Augmented Generation with LLMs (Phi, GPT, Ollama).
- **Modular Architecture**: Designed for extensibility across F#, C#, and Python microservices.

---

