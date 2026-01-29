package models

import "time"

type EmailRequest struct {
	To               string    `json:"to"`
	Subject          string    `json:"subject"`
	Timestamp        time.Time `json:"timestamp"`
	From             string    `json:"from"`
	SenderName       string    `json:"senderName"`
	FirstName        string    `json:"firstName,omitempty"`
	LastName         string    `json:"lastName,omitempty"`
	UserName         string    `json:"userName,omitempty"`
	Email            string    `json:"email,omitempty"`
	UserType         string    `json:"userType,omitempty"`
	ConfirmationLink string    `json:"confirmationLink,omitempty"`
	TemplateType     string    `json:"templateType,omitempty"`
}