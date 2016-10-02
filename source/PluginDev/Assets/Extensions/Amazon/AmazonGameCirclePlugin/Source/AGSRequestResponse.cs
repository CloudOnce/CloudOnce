/**
 * © 2012-2014 Amazon Digital Services, Inc. All rights reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"). You may not use this file except in compliance with the License. A copy
 * of the License is located at
 *
 * http://aws.amazon.com/apache2.0/
 *
 * or in the "license" file accompanying this file. This file is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
 */

/*
 * Modified by Jan Ivar Z. Carlsen.
 * Added CLOUDONCE_AMAZON build symbol.
 */

#if UNITY_ANDROID && CLOUDONCE_AMAZON

public class AGSRequestResponse {

    public string error;
    public int userData;

    protected const string PLATFORM_NOT_SUPPORTED_ERROR = "PLATFORM_NOT_SUPPORTED";
    protected const string JSON_PARSE_ERROR = "ERROR_PARSING_JSON";

    public bool IsError() {
        return !string.IsNullOrEmpty(error);
    }

}
#endif
