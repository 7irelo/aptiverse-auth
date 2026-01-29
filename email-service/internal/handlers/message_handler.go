package handlers

import (
	"fmt"
	"log"

	"aptiverse-email/internal/email"
	"aptiverse-email/internal/models"
	"aptiverse-email/internal/templates"
)

func HandleEmailMessage(emailReq *models.EmailRequest, emailSvc *email.Sender) error {
	log.Printf("Processing email for: %s", emailReq.To)
	
	var htmlBody string
	var err error
	
	switch emailReq.TemplateType {
	case "email_confirmation":
		templateData := templates.EmailTemplateData{
			FirstName:        emailReq.FirstName,
			LastName:         emailReq.LastName,
			UserName:         emailReq.UserName,
			Email:            emailReq.Email,
			UserType:         emailReq.UserType,
			ConfirmationLink: emailReq.ConfirmationLink,
		}
		htmlBody, err = templates.GenerateEmailConfirmationTemplate(templateData)
		if err != nil {
			return fmt.Errorf("failed to generate email template: %v", err)
		}
		log.Printf("Generated email confirmation template for %s", emailReq.To)
	default:
		return fmt.Errorf("no template type specified for email to %s. Available types: 'email_confirmation'", emailReq.To)
	}
	
	if err := emailSvc.Send(emailReq.To, emailReq.Subject, htmlBody); err != nil {
		return fmt.Errorf("failed to send email: %v", err)
	}
	
	log.Printf("Successfully processed email for %s", emailReq.To)
	return nil
}