# Mercury
Mercury - the messenger of the gods - a small and simple service, that takes templates, renders them, and sends emails to provided address(es).

## Prerequisites
Rest api uses .Net Core 2.1.

Core functionality projects use .Net Standart 2.0.

Service assumes that network is secure.

## Usage

* Prepare a template using desired html template language (Liquid, Handlebars, etc.);
* Prepare 'metadata.json' file with additional parameters (subject, from);
* Place a template and metadata file into some folder of desired storage (Local folder, Azure blob, etc.);
* Send request providing url-like path to the template, receivers and payload;

## Configuration

Configuration of Rest api is done via appsettings.json file.

Main sections:
* ServicesConfig.StartupSettings

Configures which implementation of a particular part of service to use.
Example:
```json
"ServicesConfig": {
      "ResourceLoader": "Local",
      "TemplateProcessor": "Liquid",
      "Emailer": "Dev"
    }
```

* MercurySettings

For now configures only global address of email sender.

* Emailer/ResourceLoaderSettings

Contains special settings for each implementation of a particular part of the service.
Example:
```json
  "EmailerSettings": {
    "SendGridSettings": {
      "ApiKey": "DummyKey"
    }
  },
  "ResourceLoaderSettings": {
    "LocalResourceSettings": {
      "RootPath": "Templates"
    }
  }
```

## Structure

Mercury provides single `Task SendAsync(EmailRequest request)` method via MercuryFacade class.

Main parts of the service:
* IResourceLoader.cs 

Loads resources - templates and metadata files.

Currently implemented: Local and AzureBlob loaders.

* IPathFinder.cs

Modifies incoming template key to point to real template and metadata files.

Currently implemented: ConstantPathFinder that appends constant file names to the key.

* ITemplateProcessor.cs

Renders the template.

Currently implemented: Liquid and Handlebars processors.

* IEmailer.cs

Sends actual email.

Currently implemented: SendGrid and Dev emailers.

## Rest Api
Mercury exposes single 'send' endpoint, and additional 'healthcheck' endpoint, that simply returns 200 OK.
### Send
Sends email(s) based on provided parameters.
* **URL**

  api/send
  
* **Method**

  `POST`
  
* **Data Params**

  **Required:**

   `tos: [string array]`
   
   `templateKey: [string]`
   
   `payload`: [object]
   
  **Optional:**
  
   `ccs: [string array]`
   
   `bccs: [string array]`
   
* **Success Response**
  * **Code:** 200 OK
  
* **Error Response**
  * **Code:** 404 NOT FOUND <br />
    **Reason:** Requested resource was not found

  OR

  * **Code:** 400 BAD REQUEST <br />
    **Reason:** Failure during template processing or request parameter is invalid
    
  OR

  * **Code:** 500 INTERNAL SERVER ERROR <br />
    **Reason:** Unexpected exception occured

### Healthcheck
Returns 200 OK.
* **URL**

  api/healthcheck
  
* **Method**

  `GET`
   
* **Success Response**
  * **Code:** 200 OK
  
* **Error Response**
  * **Code:** 500 INTERNAL SERVER ERROR <br />
    **Reason:** Unexpected exception occured

## Future Plans

* Add queue consumer(s) (e.g. RabbitMQ);
* Add caching (resources, templates);
* Load/performance testing;
* Add some more resource loaders/emailers/template processors;
* Add ability to send attachments;
    
## License
This project is licensed under the MIT License - see the [LICENSE](https://github.com/wolf8196/Mercury/blob/master/LICENSE) file for details
