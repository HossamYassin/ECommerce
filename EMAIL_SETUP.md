# Email Service Setup Guide

This guide explains how to configure the email service for sending real emails.

## Implementation

The application uses **MailKit** with SMTP to send emails. This is the easiest and most flexible solution that works with any SMTP server.

## Configuration Options

### Option 1: Gmail (Easiest for Development)

1. **Enable 2-Step Verification** on your Google account
2. **Generate an App Password**:
   - Go to Google Account Settings → Security
   - Enable 2-Step Verification if not already enabled
   - Click "App passwords"
   - Generate a new app password for "Mail"
   - Copy the 16-character password

3. **Update `appsettings.Development.json`**:
```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUsername": "your-email@gmail.com",
    "SmtpPassword": "your-16-char-app-password",
    "FromEmail": "your-email@gmail.com",
    "FromName": "E-Commerce Team",
    "EnableSsl": true
  }
}
```

### Option 2: Outlook/Hotmail

```json
{
  "EmailSettings": {
    "SmtpServer": "smtp-mail.outlook.com",
    "SmtpPort": 587,
    "SmtpUsername": "your-email@outlook.com",
    "SmtpPassword": "your-password",
    "FromEmail": "your-email@outlook.com",
    "FromName": "E-Commerce Team",
    "EnableSsl": true
  }
}
```

### Option 3: Yahoo Mail

```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.mail.yahoo.com",
    "SmtpPort": 587,
    "SmtpUsername": "your-email@yahoo.com",
    "SmtpPassword": "your-app-password",
    "FromEmail": "your-email@yahoo.com",
    "FromName": "E-Commerce Team",
    "EnableSsl": true
  }
}
```

### Option 4: Custom SMTP Server

For custom SMTP servers (e.g., SendGrid, Mailgun, AWS SES, etc.):

```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.your-provider.com",
    "SmtpPort": 587,
    "SmtpUsername": "your-smtp-username",
    "SmtpPassword": "your-smtp-password",
    "FromEmail": "noreply@yourdomain.com",
    "FromName": "E-Commerce Team",
    "EnableSsl": true
  }
}
```

## Common SMTP Providers

### SendGrid
- **SMTP Server**: `smtp.sendgrid.net`
- **Port**: `587`
- **Username**: `apikey`
- **Password**: Your SendGrid API Key
- **From Email**: Verified sender email in SendGrid

### Mailgun
- **SMTP Server**: `smtp.mailgun.org`
- **Port**: `587`
- **Username**: Your Mailgun SMTP username
- **Password**: Your Mailgun SMTP password

### AWS SES
- **SMTP Server**: `email-smtp.{region}.amazonaws.com` (e.g., `email-smtp.us-east-1.amazonaws.com`)
- **Port**: `587`
- **Username**: Your AWS SES SMTP username
- **Password**: Your AWS SES SMTP password

## Security Notes

⚠️ **Important**: Never commit real passwords to version control!

1. Use **User Secrets** for development:
```bash
dotnet user-secrets set "EmailSettings:SmtpPassword" "your-password"
dotnet user-secrets set "EmailSettings:SmtpUsername" "your-username"
```

2. Use **Environment Variables** or **Azure Key Vault** in production:
```bash
export EmailSettings__SmtpPassword="your-password"
export EmailSettings__SmtpUsername="your-username"
```

3. For production, remove sensitive values from `appsettings.json` and use secure storage.

## Testing

Once configured, the email service will automatically send emails when:
- Orders are placed
- Orders are cancelled

The service will log warnings if email settings are not configured (for development/testing scenarios).

## Troubleshooting

### "Authentication failed" error:
- Verify your username and password are correct
- For Gmail, make sure you're using an App Password, not your regular password
- Check if 2-Step Verification is enabled (required for App Passwords)

### "Connection timeout" error:
- Check if port 587 is blocked by firewall
- Verify SMTP server address is correct
- Try port 465 with SSL instead (requires different configuration)

### Emails not sending:
- Check application logs for error messages
- Verify EmailSettings are configured correctly
- Test SMTP connection using a tool like `telnet` or `openssl`

## Alternative: Using User Secrets (Recommended for Development)

Instead of storing credentials in `appsettings.Development.json`, use User Secrets:

```bash
cd ECommerce.API
dotnet user-secrets init
dotnet user-secrets set "EmailSettings:SmtpServer" "smtp.gmail.com"
dotnet user-secrets set "EmailSettings:SmtpPort" "587"
dotnet user-secrets set "EmailSettings:SmtpUsername" "your-email@gmail.com"
dotnet user-secrets set "EmailSettings:SmtpPassword" "your-app-password"
dotnet user-secrets set "EmailSettings:FromEmail" "your-email@gmail.com"
dotnet user-secrets set "EmailSettings:FromName" "E-Commerce Team"
dotnet user-secrets set "EmailSettings:EnableSsl" "true"
```

This keeps your credentials out of version control!

