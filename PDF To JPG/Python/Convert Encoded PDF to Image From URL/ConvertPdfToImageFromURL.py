import requests # pip install requests

# The authentication key (API Key).
# Get your own by registering at https://app.pdf.co
API_KEY = "********************************"

# Base URL for PDF.co Web API requests
BASE_URL = "https://api.pdf.co/v1"

# Direct URL of encoded source PDF file. This PDF is Encrypted with AES128 algorithm.
# You can also upload your own file into PDF.co and use it as url. Check "Upload File" samples for code snippets: https://github.com/bytescout/pdf-co-api-samples/tree/master/File%20Upload/    
SourceFileURL = "https://bytescout-com.s3-us-west-2.amazonaws.com/files/demo-files/cloud-api/encryption/sample_encrypted_aes128.pdf"

# Comma-separated list of page indices (or ranges) to process. Leave empty for all pages. Example: '0,2-5,7-'.
Pages = ""

# Refer to documentations for more info. https://apidocs.pdf.co/32-1-user-controlled-data-encryption-and-decryption
Profiles = "{ 'DataDecryptionAlgorithm': 'AES128', 'DataDecryptionKey': 'HelloThisKey1234', 'DataDecryptionIV': 'TreloThisKey1234' }"

def main(args = None):
    convertPdfToImage(SourceFileURL)


def convertPdfToImage(sourceFileURL):
    """Converts PDF To Image using PDF.co Web API"""

    # Prepare requests params as JSON
    # See documentation: https://apidocs.pdf.co
    parameters = {}
    parameters["url"] = sourceFileURL
    parameters["pages"] = Pages
    parameters["profiles"] = Profiles

    # Prepare URL for 'PDF To JPG' API request
    url = "{}/pdf/convert/to/jpg".format(BASE_URL)

    # Execute request and get response as JSON
    response = requests.post(url, data=parameters, headers={ "x-api-key": API_KEY })
    if (response.status_code == 200):
        json = response.json()

        if json["error"] == False:

            # Download generated JPG files
            part = 1

            for resultFileUrl in json["urls"]:
                # Download Result File
                r = requests.get(resultFileUrl, stream=True)

                localFileUrl = f"Page{part}.jpg"

                if r.status_code == 200:
                    with open(localFileUrl, 'wb') as file:
                        for chunk in r:
                            file.write(chunk)
                    print(f"Result file saved as \"{localFileUrl}\" file.")
                else:
                    print(f"Request error: {response.status_code} {response.reason}")

                part = part + 1
        else:
            # Show service reported error
            print(json["message"])
    else:
        print(f"Request error: {response.status_code} {response.reason}")


if __name__ == '__main__':
    main()