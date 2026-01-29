# 💳 Aptiverse Payments Microservice

A robust, secure payment gateway microservice built with Ruby on Rails, designed to handle all financial transactions for the Aptiverse educational platform. Supports multiple payment methods, subscription management, and secure transaction processing through Stripe.

## 🚀 Overview

This service manages the complete payment lifecycle for Aptiverse, including:
- **Subscription management** for Freemium, Student, Family, and School plans
- **One-time payments** for tutor sessions and specialized courses
- **Bursary and financial aid** payment processing
- **Multi-tenant billing** for schools and families
- **Payment analytics** and reporting

## 🛠️ Technology Stack

### Core Framework
- **Ruby on Rails 7+** - Main application framework
- **PostgreSQL** - Primary database for transaction records
- **Redis** - Caching and background job processing

### Payment & Financial
- **Stripe** - Primary payment processor
- **Stripe Billing** - Subscription management
- **Stripe Connect** - For tutor payouts

### Background Processing
- **Sidekiq** - Background job processing
- **Sidekiq Cron** - Scheduled tasks (subscription renewals, invoice generation)

### API & Communication
- **RESTful JSON API** - Internal service communication
- **RabbitMQ** - Async messaging with other Aptiverse services
- **gRPC** - High-performance internal communication (optional)

### Monitoring & Security
- **Rack Attack** - Rate limiting and security
- **Sentry** - Error monitoring and tracking
- **Audited** - Audit trails for financial transactions
- **Pundit** - Authorization policies

## 📦 Key Features

### Payment Processing
- 💳 Credit/debit card payments via Stripe
- 🔄 Subscription management with automatic renewals
- 🏦 EFT/bank transfer support for South African banks
- 📱 Mobile money integration (M-Pesa, etc.)
- 💰 Multiple currency support (ZAR, USD, EUR)

### Subscription Management
- Tiered pricing plans (Freemium, Student, Family, School)
- Prorated upgrades/downgrades
- Trial period management
- Grace periods for failed payments

### Financial Operations
- Automated invoice generation
- Receipt delivery via email
- Refund processing
- Dispute management
- Payouts to tutors and service providers

### Security & Compliance
- PCI DSS compliant through Stripe
- Secure webhook handling
- Payment data encryption at rest
- Audit trails for all financial operations
- GDPR-compliant data handling

## 🏗️ Architecture

```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│   Main API      │    │   Payment Service │    │    Stripe       │
│   (.NET)        │◄──►│   (Rails)         │◄──►│    Gateway      │
└─────────────────┘    └──────────────────┘    └─────────────────┘
         │                       │                       │
         ▼                       ▼                       ▼
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│   Auth Service  │    │   RabbitMQ       │    │   PostgreSQL    │
│   (.NET)        │    │   (Messages)     │    │   (Database)    │
└─────────────────┘    └──────────────────┘    └─────────────────┘
```

## 🔧 API Endpoints

### Subscriptions
```
POST   /api/v1/subscriptions          # Create subscription
GET    /api/v1/subscriptions/:id      # Get subscription details
PUT    /api/v1/subscriptions/:id      # Update subscription
DELETE /api/v1/subscriptions/:id      # Cancel subscription
POST   /api/v1/subscriptions/:id/reactivate  # Reactivate subscription
```

### Payments
```
POST   /api/v1/payments               # Process one-time payment
GET    /api/v1/payments/:id           # Payment status
POST   /api/v1/payments/:id/refund    # Process refund
```

### Invoices
```
GET    /api/v1/invoices               # List invoices
GET    /api/v1/invoices/:id           # Invoice details
POST   /api/v1/invoices/:id/retry     # Retry failed payment
```

### Webhooks
```
POST   /api/v1/webhooks/stripe        # Stripe webhook handler
```

## ⚙️ Configuration

### Environment Variables
```ruby
STRIPE_SECRET_KEY=sk_live_...
STRIPE_PUBLISHABLE_KEY=pk_live_...
STRIPE_WEBHOOK_SECRET=whsec_...

DATABASE_URL=postgresql://user:pass@localhost:5432/aptiverse_payments
REDIS_URL=redis://localhost:6379/0

RABBITMQ_URL=amqp://guest:guest@localhost:5672

SENTRY_DSN=https://...
```

### Stripe Configuration
```ruby
# config/initializers/stripe.rb
Stripe.api_key = ENV['STRIPE_SECRET_KEY']
Stripe.api_version = '2023-10-16'

StripeEvent.configure do |events|
  events.subscribe 'invoice.payment_succeeded', PaymentSucceededHandler.new
  events.subscribe 'invoice.payment_failed', PaymentFailedHandler.new
  events.subscribe 'customer.subscription.updated', SubscriptionUpdatedHandler.new
end
```

## 🚀 Getting Started

### Prerequisites
- Ruby 3.2+
- PostgreSQL 14+
- Redis 6+
- Stripe account

### Installation
```bash
# Clone repository
git clone https://github.com/aptiverse/payments-service.git
cd payments-service

# Install dependencies
bundle install

# Setup database
rails db:create
rails db:migrate

# Run services
rails server
sidekiq -C config/sidekiq.yml
```

### Docker Setup
```bash
docker-compose up -d
docker-compose run web rails db:create db:migrate
```

## 🔒 Security Features

- Rate limiting on all endpoints
- Webhook signature verification
- SQL injection prevention
- XSS protection
- CSRF protection
- Secure headers configuration
- Payment data never stored locally
- Regular security audits

## 📊 Monitoring & Logging

- Structured JSON logging
- Performance metrics with New Relic
- Error tracking with Sentry
- Background job monitoring with Sidekiq Web UI
- Payment success/failure analytics
- Custom dashboards for financial metrics

## 🤝 Integration with Aptiverse

### Event Publishing
```ruby
# Example: Payment successful event
RabbitMQ.publish('payments.payment_succeeded', {
  user_id: user.id,
  amount: payment.amount,
  plan_type: subscription.plan_type,
  timestamp: Time.current
})
```

### Consumed Events
- `users.user_created` - Create Stripe customer
- `billing.plan_changed` - Update subscription
- `schools.school_registered` - Setup school billing

## 🧪 Testing

```bash
# Run test suite
bundle exec rspec

# Test coverage
bundle exec rspec --coverage

# Webhook testing with Stripe CLI
stripe listen --forward-to localhost:3000/api/v1/webhooks/stripe
stripe trigger invoice.payment_succeeded
```

## 📈 Deployment

### Production Requirements
- HTTPS enforcement
- Webhook endpoint configuration in Stripe dashboard
- Regular database backups
- Monitoring and alerting setup
- PCI compliance review

### Health Checks
```
GET /health          # Application health
GET /health/db       # Database connectivity
GET /health/redis    # Redis connectivity
GET /health/stripe   # Stripe API status
```

## 🎯 Future Enhancements

- [ ] Multiple payment gateway support (PayFast, PayPal)
- [ ] Advanced revenue recognition
- [ ] Tax calculation (VAT, etc.)
- [ ] Payment plan support for bursaries
- [ ] Advanced analytics and reporting
- [ ] Mobile SDK integration

## 📄 License

This project is part of the Aptiverse educational ecosystem. All rights reserved.

---

**Built with ❤️ for accessible education**
