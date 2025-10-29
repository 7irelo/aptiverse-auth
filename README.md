# 📚 Aptiverse API – AI-Powered Educational Ecosystem
<img width="1916" height="908" alt="image" src="https://github.com/user-attachments/assets/1b8c5117-eb54-426a-bf2b-d1c0fee7c50c" />

**Aptiverse** is an intelligent, AI-driven educational API built with the mission of transforming the way South African students in Grades 11 & 12 learn, grow, and thrive—regardless of background or school affiliation.

> 🔧 This repository contains the **backend API** powering the Aptiverse platform, built using **.NET 8 Core**, **Redis**, **AWS**, and other modern cloud-native tools and practices.

---

## 🚀 Overview

Aptiverse is not just an LMS or digital study tool. It is a **three-part educational ecosystem** built to support:

* **Students** with personalized, emotionally intelligent study support.
* **Teachers** with actionable insights on learner performance.
* **Parents** with optional access to monitor their child's academic and emotional wellbeing.

Unlike traditional systems locked behind school subscriptions, **Aptiverse empowers all students**, regardless of whether their school is onboarded.

---

## 🧠 Core Features

### 🎓 Student-Centric Workspace

* AI-powered **study companion** that learns and adapts to individual learning styles.
* Personal workspaces with note-taking, to-do lists, and smart reminders.
* Gamified **goal system** with merit-based rewards.

### 💬 Emotional Intelligence Engine

* Learner diary entries are interpreted using psychological AI models.
* Detects emotional struggles and suggests actionable mental wellness strategies.

### 📘 Subject-Specific AI Tutors

* Each subject has a dedicated FastAPI model trained to assess performance.
* Auto-generates practice material, tips, and explanations tailored to the learner.

### 👨‍🏫 Teacher & Parent Insights

* Teachers view strengths, gaps, and growth curves across subjects.
* Parents can opt-in to receive progress updates, respecting student privacy.

### 🏆 Rewards & Access Equality

* Students unlock premium features by achieving goals.
* Non-subscribed school learners can still access advanced tools via performance merit.

---

## 🛠️ Tech Stack

| Layer                       | Technology                            |
| --------------------------- | ------------------------------------- |
| **API**                     | .NET 8 Core, C#, REST, Docker         |
| **AI Models**               | Python, FastAPI                       |
| **Auth**                    | AWS Cognito                           |
| **Database**                | Amazon RDS (PostgreSQL)               |
| **Caching**                 | Redis on EC2                          |
| **Message Queue**           | RabbitMQ (optional)                   |
| **Benchmarking**            | Crank                                 |
| **API Communication**       | RestSharp                             |
| **DevOps**                  | Docker, CI/CD, AWS Lambda, CloudWatch |
| **Frontend (context only)** | Next.js, TypeScript, Turbopack        |

---

## 📂 Project Structure

```
Aptiverse.Api.Web           → Entry-point Web API, Controllers, Config
Aptiverse.Application       → Application layer: DTOs, Services, Mappings
Aptiverse.Benchmarks        → Performance testing with Crank
Aptiverse.Core              → Core domain logic, business rules, services
Aptiverse.Domain            → Entities and domain-driven design
Aptiverse.Infrastructure    → Data, Repositories, EF Migrations, Utilities
```

> Advanced C# features like **Reflection**, **Dependency Injection**, and custom **Repository Patterns** are used extensively to keep code clean, testable, and scalable.

---

## 🧪 Benchmarking

Performance is critical for an AI-integrated system. Aptiverse uses **[Crank](https://github.com/dotnet/crank)** to benchmark API throughput, latency, and concurrency under simulated student and teacher loads.

---

## 🧠 Future Ideas

Here are some features and modules that can be added next:

* ✍️ **Essay Evaluator** using GPT-style LLMs for literature and history
* 🔗 **Open Education Resource Integration** (e.g., Siyavula, Khan Academy)
* 🧬 **Career Pathway Predictor** based on strengths, interests, and behavior
* 🏫 **School Management Module**: Class schedules, announcements, and parent-teacher meetings
* 📱 **Offline-first Mobile App** for learners with low connectivity
* 🔒 **Blockchain-based Certification** and academic recordkeeping

---

## 🔐 Security & Privacy

* **User Authentication** via AWS Cognito (JWT-based).
* Fine-grained **privacy controls** for student data visibility.
* Strict **rate limiting** and **input sanitization** for API endpoints.

---

## 🧑‍💻 Developer Notes

* REST APIs are modular and organized by domain and service area.
* **Dockerfile** included for containerized deployment.
* AWS Lambda integration for on-demand compute and AI model inference.
* Redis and PostgreSQL configured for scalability and caching.

---

## 📬 Contribution & Collaboration

If you're passionate about AI in education, student empowerment, or scaling innovation in Africa, we’d love your input! Reach out via [LinkedIn](#) or contribute to the repo through PRs or ideas.

---

## 📢 License

> This project is currently in active development and may be private, licensed, or open-source in future iterations.

---

### ❤️ Aptiverse: For Every Learner

Aptiverse is more than just a school tool—it's a **resilient platform for growth**, ensuring no student is left behind due to lack of access, support, or personalized guidance.

---
