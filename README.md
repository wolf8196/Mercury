# Mercury
Mercury - the messenger of the gods - a small and simple service, that takes templates, renders them, and sends emails to provided address(es).

## Prerequisites
Api & messaging uses .NET Core 3.1.

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
* ServiceSettings

Configures which implementation of a particular part of service to use.
Example:
```json
"ServiceSettings": {
      "ResourceLoader": "Local",
      "TemplateProcessor": "Liquid",
      "Emailer": "Dev"
    }
```

* MercurySettings

For now configures only global address of email sender.

* RabbitSettings

Configures main properties required to queue messages into RabbitMq.

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

Currently implemented: SendGrid, Smtp and Mock emailers.

## Api
### Send
Sends email immediately.
* **URL**

  api/v1/send
  
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
  * **Body:**
  ```json
  {
    "success": true,
    "status": 200,
    "errors": []
  }
  ```
  
* **Error Response**
  * **Code:** 400 BAD REQUEST <br />
    **Body:**
    ```json
    {
        "success": false,
        "status": 400,
        "errors": [
            {
                "message": "Error message.",
                "metadata": {},
                "reasons": [
                    {
                        "message": "Reason message.",
                        "metadata": {},
                        "reasons": []
                    }
                ]
            }
        ]
    }
    ```
    
  OR

  * **Code:** 500 INTERNAL SERVER ERROR <br />
    **Body:** Unexpected exception occured
    ```json
    {
        "success": false,
        "status": 500,
        "errors": [
            {
                "message": "Unhandled error occured.",
                "metadata": {},
                "reasons": []
            }
        ]
    }
    ```



### Queue
Publishes email into queue for later processing.
* **URL**

  api/v1/queue
  
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
  * **Code:** 202 ACCEPTED
  * **Body:**
  ```json
  {
    "success": true,
    "status": 202,
    "errors": []
  }
  ```
  
* **Error Response**
  * **Code:** 400 BAD REQUEST <br />
    **Body:**
    ```json
    {
        "success": false,
        "status": 400,
        "errors": [
            {
                "message": "Error message.",
                "metadata": {},
                "reasons": [
                    {
                        "message": "Reason message.",
                        "metadata": {},
                        "reasons": []
                    }
                ]
            }
        ]
    }
    ```
    
  OR

  * **Code:** 500 INTERNAL SERVER ERROR <br />
    **Body:** Unexpected exception occured
    ```json
    {
        "success": false,
        "status": 500,
        "errors": [
            {
                "message": "Unhandled error occured.",
                "metadata": {},
                "reasons": []
            }
        ]
    }
    ```


### Healthcheck
Returns 200 OK.
* **URL**

  api/v1/healthcheck
  
* **Method**

  `GET`
   
* **Success Response**
  * **Code:** 200 ACCEPTED
  * **Body:**
  ```json
  {
    "success": true,
    "status": 200,
    "errors": []
  }
  ```

  * **Error Response**
  * **Code:** 500 INTERNAL SERVER ERROR <br />
    **Body:** Unexpected exception occured
    ```json
    {
        "success": false,
        "status": 500,
        "errors": [
            {
                "message": "Unhandled error occured.",
                "metadata": {},
                "reasons": []
            }
        ]
    }
    ```
    
## License
This project is licensed under the MIT License - see the [LICENSE](https://github.com/wolf8196/Mercury/blob/master/LICENSE) file for details
