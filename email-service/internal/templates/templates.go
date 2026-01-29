package templates

import (
	"html/template"
	"strings"
	"time"
)

type EmailTemplateData struct {
	FirstName        string
	LastName         string
	UserName         string
	Email            string
	UserType         string
	ConfirmationLink string
	CurrentYear      int
}

func GenerateEmailConfirmationTemplate(data EmailTemplateData) (string, error) {
    const templateStr = 
        `<!DOCTYPE html>
        <html lang='en'>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <title>Confirm Your Email - Aptiverse</title>
            <style>
                @import url('https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700&display=swap');
                
                * {
                    margin: 0;
                    padding: 0;
                    box-sizing: border-box;
                }
                
                body {
                    font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
                    line-height: 1.7;
                    color: #1f2937;
                    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                    margin: 0;
                    padding: 20px;
                    min-height: 100vh;
                }
                
                .container {
                    max-width: 580px;
                    margin: 0 auto;
                    background: #ffffff;
                    border-radius: 20px;
                    overflow: hidden;
                    box-shadow: 
                        0 20px 40px rgba(0, 0, 0, 0.1),
                        0 4px 12px rgba(0, 0, 0, 0.08);
                    transition: transform 0.3s ease, box-shadow 0.3s ease;
                }
                
                .container:hover {
                    transform: translateY(-2px);
                    box-shadow: 
                        0 25px 50px rgba(0, 0, 0, 0.15),
                        0 8px 20px rgba(0, 0, 0, 0.12);
                }
                
                .header {
                    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                    color: white;
                    padding: 50px 40px 40px;
                    text-align: center;
                    position: relative;
                    overflow: hidden;
                }
                
                .header::before {
                    content: '';
                    position: absolute;
                    top: 0;
                    left: 0;
                    right: 0;
                    bottom: 0;
                    background: url("data:image/svg+xml,%3Csvg width='60' height='60' viewBox='0 0 60 60' xmlns='http://www.w3.org/2000/svg'%3E%3Cg fill='none' fill-rule='evenodd'%3E%3Cg fill='%23ffffff' fill-opacity='0.05'%3E%3Ccircle cx='30' cy='30' r='2'/%3E%3C/g%3E%3C/g%3E%3C/svg%3E");
                }
                
                .header-content {
                    position: relative;
                    z-index: 2;
                }
                
                .logo {
                    font-size: 48px;
                    margin-bottom: 16px;
                    display: block;
                }
                
                .header h1 {
                    margin: 0;
                    font-size: 32px;
                    font-weight: 700;
                    letter-spacing: -0.5px;
                    text-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
                }
                
                .header-subtitle {
                    font-size: 16px;
                    opacity: 0.9;
                    margin-top: 8px;
                    font-weight: 400;
                }
                
                .content {
                    padding: 50px 40px;
                }
                
                .welcome-section {
                    text-align: center;
                    margin-bottom: 40px;
                }
                
                .welcome-text {
                    font-size: 20px;
                    font-weight: 500;
                    color: #374151;
                    margin-bottom: 16px;
                }
                
                .welcome-name {
                    color: #667eea;
                    font-weight: 600;
                }
                
                .intro-text {
                    color: #6b7280;
                    font-size: 16px;
                    line-height: 1.6;
                }
                
                .user-info-card {
                    background: linear-gradient(135deg, #f8fafc 0%, #f1f5f9 100%);
                    border: 1px solid #e2e8f0;
                    border-radius: 16px;
                    padding: 24px;
                    margin: 32px 0;
                    position: relative;
                }
                
                .user-info-card::before {
                    content: '';
                    position: absolute;
                    top: 0;
                    left: 0;
                    width: 4px;
                    height: 100%;
                    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                    border-radius: 4px 0 0 4px;
                }
                
                .user-info-title {
                    font-size: 18px;
                    font-weight: 600;
                    color: #1f2937;
                    margin-bottom: 16px;
                    display: flex;
                    align-items: center;
                    gap: 8px;
                }
                
                .user-info-title::before {
                    content: '👤';
                    font-size: 20px;
                }
                
                .user-details {
                    display: grid;
                    grid-template-columns: 1fr;
                    gap: 12px;
                }
                
                .user-detail {
                    display: flex;
                    justify-content: space-between;
                    align-items: center;
                    padding: 8px 0;
                    border-bottom: 1px solid #f1f5f9;
                }
                
                .user-detail:last-child {
                    border-bottom: none;
                }
                
                .detail-label {
                    color: #6b7280;
                    font-weight: 500;
                }
                
                .detail-value {
                    color: #1f2937;
                    font-weight: 600;
                }
                
                .steps-container {
                    margin: 40px 0;
                }
                
                .steps-title {
                    font-size: 18px;
                    font-weight: 600;
                    color: #1f2937;
                    margin-bottom: 24px;
                    text-align: center;
                }
                
                .steps {
                    display: grid;
                    grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
                    gap: 20px;
                    margin-bottom: 32px;
                }
                
                .step {
                    text-align: center;
                    padding: 20px;
                    background: #f8fafc;
                    border-radius: 12px;
                    transition: transform 0.3s ease, background 0.3s ease;
                }
                
                .step:hover {
                    transform: translateY(-4px);
                    background: #ffffff;
                    box-shadow: 0 8px 20px rgba(0, 0, 0, 0.08);
                }
                
                .step-number {
                    width: 40px;
                    height: 40px;
                    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                    color: white;
                    border-radius: 50%;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    font-weight: 700;
                    font-size: 18px;
                    margin: 0 auto 12px;
                    box-shadow: 0 4px 12px rgba(102, 126, 234, 0.3);
                }
                
                .step-text {
                    color: #4b5563;
                    font-size: 14px;
                    font-weight: 500;
                    line-height: 1.4;
                }
                
                .confirmation-section {
                    text-align: center;
                    margin: 40px 0;
                }
                
                .confirmation-button {
                    display: inline-block;
                    background: linear-gradient(135deg, #10b981 0%, #059669 100%);
                    color: white;
                    padding: 18px 40px;
                    text-decoration: none;
                    border-radius: 50px;
                    font-weight: 600;
                    font-size: 16px;
                    transition: all 0.3s ease;
                    box-shadow: 
                        0 4px 15px rgba(16, 185, 129, 0.3),
                        0 8px 25px rgba(16, 185, 129, 0.2);
                    position: relative;
                    overflow: hidden;
                }
                
                .confirmation-button::before {
                    content: '';
                    position: absolute;
                    top: 0;
                    left: -100%;
                    width: 100%;
                    height: 100%;
                    background: linear-gradient(90deg, transparent, rgba(255,255,255,0.3), transparent);
                    transition: left 0.5s ease;
                }
                
                .confirmation-button:hover {
                    transform: translateY(-2px);
                    box-shadow: 
                        0 6px 20px rgba(16, 185, 129, 0.4),
                        0 12px 30px rgba(16, 185, 129, 0.3);
                }
                
                .confirmation-button:hover::before {
                    left: 100%;
                }
                
                .alternative-section {
                    margin: 32px 0;
                    text-align: center;
                }
                
                .alternative-text {
                    color: #6b7280;
                    font-size: 14px;
                    margin-bottom: 16px;
                }
                
                .confirmation-link {
                    background: #f8fafc;
                    border: 1px solid #e5e7eb;
                    border-radius: 12px;
                    padding: 16px;
                    word-break: break-all;
                    font-family: 'Monaco', 'Menlo', 'Ubuntu Mono', monospace;
                    font-size: 12px;
                    color: #374151;
                    transition: all 0.3s ease;
                    cursor: text;
                }
                
                .confirmation-link:hover {
                    background: #f1f5f9;
                    border-color: #d1d5db;
                }
                
                .support-info {
                    background: linear-gradient(135deg, #fef3c7 0%, #fef7cd 100%);
                    border: 1px solid #fcd34d;
                    border-radius: 16px;
                    padding: 24px;
                    margin: 32px 0;
                    text-align: center;
                }
                
                .support-title {
                    font-size: 16px;
                    font-weight: 600;
                    color: #92400e;
                    margin-bottom: 12px;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    gap: 8px;
                }
                
                .support-title::before {
                    content: '⚠️';
                }
                
                .support-text {
                    color: #92400e;
                    font-size: 14px;
                    line-height: 1.5;
                    margin-bottom: 8px;
                }
                
                .footer {
                    background: #1f2937;
                    color: #9ca3af;
                    padding: 40px;
                    text-align: center;
                }
                
                .footer-content {
                    max-width: 400px;
                    margin: 0 auto;
                }
                
                .footer-logo {
                    font-size: 24px;
                    margin-bottom: 16px;
                    display: block;
                }
                
                .footer-text {
                    font-size: 14px;
                    line-height: 1.5;
                    margin-bottom: 8px;
                }
                
                .footer-link {
                    color: #667eea;
                    text-decoration: none;
                    transition: color 0.3s ease;
                }
                
                .footer-link:hover {
                    color: #764ba2;
                }
                
                .copyright {
                    font-size: 12px;
                    opacity: 0.7;
                    margin-top: 16px;
                }
                
                /* Responsive Design */
                @media (max-width: 600px) {
                    body {
                        padding: 10px;
                    }
                    
                    .container {
                        border-radius: 16px;
                    }
                    
                    .header {
                        padding: 40px 24px 32px;
                    }
                    
                    .header h1 {
                        font-size: 28px;
                    }
                    
                    .content {
                        padding: 40px 24px;
                    }
                    
                    .steps {
                        grid-template-columns: 1fr;
                    }
                    
                    .confirmation-button {
                        padding: 16px 32px;
                        font-size: 15px;
                    }
                }
                
                /* Dark mode support */
                @media (prefers-color-scheme: dark) {
                    .container {
                        background: #1f2937;
                        color: #f9fafb;
                    }
                    
                    .welcome-text, .detail-value {
                        color: #f9fafb;
                    }
                    
                    .intro-text, .detail-label, .step-text, .alternative-text {
                        color: #d1d5db;
                    }
                    
                    .user-info-card {
                        background: #374151;
                        border-color: #4b5563;
                    }
                    
                    .step {
                        background: #374151;
                    }
                    
                    .confirmation-link {
                        background: #374151;
                        border-color: #4b5563;
                        color: #d1d5db;
                    }
                }
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <div class='header-content'>
                        <div class='logo'>🚀</div>
                        <h1>Welcome to Aptiverse!</h1>
                        <div class='header-subtitle'>Your learning journey begins here</div>
                    </div>
                </div>

                <div class='content'>
                    <div class='welcome-section'>
                        <p class='welcome-text'>Hello <span class='welcome-name'>{{.FirstName}} {{.LastName}}</span>,</p>
                        <p class='intro-text'>
                            Welcome to Aptiverse! We're thrilled to have you join our community of learners and educators. 
                            To get started and unlock all the amazing features, please confirm your email address.
                        </p>
                    </div>

                    <div class='user-info-card'>
                        <div class='user-info-title'>Account Information</div>
                        <div class='user-details'>
                            <div class='user-detail'>
                                <span class='detail-label'>Username:</span>
                                <span class='detail-value'>{{.UserName}}</span>
                            </div>
                            <div class='user-detail'>
                                <span class='detail-label'>Email:</span>
                                <span class='detail-value'>{{.Email}}</span>
                            </div>
                            <div class='user-detail'>
                                <span class='detail-label'>Account Type:</span>
                                <span class='detail-value'>{{.UserType}}</span>
                            </div>
                        </div>
                    </div>

                    <div class='steps-container'>
                        <div class='steps-title'>Get Started in 3 Simple Steps</div>
                        <div class='steps'>
                            <div class='step'>
                                <div class='step-number'>1</div>
                                <div class='step-text'>Click the confirmation button below</div>
                            </div>
                            <div class='step'>
                                <div class='step-number'>2</div>
                                <div class='step-text'>Verify your email address</div>
                            </div>
                            <div class='step'>
                                <div class='step-number'>3</div>
                                <div class='step-text'>Start your learning journey!</div>
                            </div>
                        </div>
                    </div>

                    <div class='confirmation-section'>
                        <a href='{{.ConfirmationLink}}' class='confirmation-button'>
                            Confirm Email Address
                        </a>
                    </div>

                    <div class='alternative-section'>
                        <p class='alternative-text'>
                            If the button doesn't work, copy and paste this link into your browser:
                        </p>
                        <div class='confirmation-link' onclick='this.select(); document.execCommand("copy");'>
                            {{.ConfirmationLink}}
                        </div>
                        <p style='color: #6b7280; font-size: 12px; margin-top: 8px;'>
                            Click to copy • Link expires in 24 hours
                        </p>
                    </div>

                    <div class='support-info'>
                        <div class='support-title'>Need Assistance?</div>
                        <p class='support-text'>
                            If you didn't create this account or need help, please contact our support team immediately.
                        </p>
                        <p class='support-text'>
                            For security reasons, this confirmation link will expire in 24 hours.
                        </p>
                    </div>
                </div>

                <div class='footer'>
                    <div class='footer-content'>
                        <div class='footer-logo'>✨</div>
                        <p class='footer-text'>
                            Transforming education through innovative technology and collaborative learning.
                        </p>
                        <p class='footer-text'>
                            Have questions? <a href='https://aptiverse.co.za/support' class='footer-link'>Contact Support</a>
                        </p>
                        <p class='copyright'>
                            &copy; {{.CurrentYear}} Aptiverse. All rights reserved.<br>
                            This is an automated message, please do not reply to this email.
                        </p>
                    </div>
                </div>
            </div>

            <script>
                // Simple copy functionality
                document.addEventListener('DOMContentLoaded', function() {
                    const linkElement = document.querySelector('.confirmation-link');
                    if (linkElement) {
                        linkElement.addEventListener('click', function() {
                            const range = document.createRange();
                            range.selectNodeContents(this);
                            const selection = window.getSelection();
                            selection.removeAllRanges();
                            selection.addRange(range);
                            
                            try {
                                document.execCommand('copy');
                                const originalText = this.textContent;
                                this.textContent = 'Link copied to clipboard!';
                                this.style.background = '#10b981';
                                this.style.color = 'white';
                                
                                setTimeout(() => {
                                    this.textContent = originalText;
                                    this.style.background = '';
                                    this.style.color = '';
                                }, 2000);
                            } catch (err) {
                                console.log('Copy failed:', err);
                            }
                            
                            selection.removeAllRanges();
                        });
                    }
                });
            </script>
        </body>
        </html>`

    tmpl, err := template.New("email").Parse(templateStr)
    if err != nil {
        return "", err
    }

    var result strings.Builder
    data.CurrentYear = time.Now().Year()
    
    err = tmpl.Execute(&result, data)
    if err != nil {
        return "", err
    }

    return result.String(), nil
}