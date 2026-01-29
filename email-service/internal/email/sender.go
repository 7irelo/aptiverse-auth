package email

import (
	"fmt"
	"log"
	"net/smtp"
	"net/mail"

	"aptiverse-email/internal/config"
)

type Sender struct {
	config *config.Config
}

func NewSender(cfg *config.Config) *Sender {
	return &Sender{config: cfg}
}

func (s *Sender) Send(to, subject, htmlBody string) error {
	auth := smtp.PlainAuth("", 
		s.config.SMTP.Username, 
		s.config.SMTP.Password, 
		s.config.SMTP.Host,
	)

	recipients := []string{to}
	from := s.config.SMTP.From
	if from == "" {
		from = s.config.SMTP.Username
	}

	if _, err := mail.ParseAddress(from); err != nil {
		return fmt.Errorf("invalid from address: %v", err)
	}
	if _, err := mail.ParseAddress(to); err != nil {
		return fmt.Errorf("invalid to address: %v", err)
	}

	headers := make(map[string]string)
	headers["From"] = from
	headers["To"] = to
	headers["Subject"] = subject
	headers["MIME-Version"] = "1.0"
	headers["Content-Type"] = "text/html; charset=\"UTF-8\""

	message := ""
	for k, v := range headers {
		message += fmt.Sprintf("%s: %s\r\n", k, v)
	}
	message += "\r\n" + htmlBody

	err := smtp.SendMail(
		s.config.SMTP.Host+":"+s.config.SMTP.Port,
		auth,
		from,
		recipients,
		[]byte(message),
	)

	if err != nil {
		log.Printf("Failed to send email to %s: %v", to, err)
		return err
	}

	log.Printf("Email sent successfully to %s", to)
	return nil
}