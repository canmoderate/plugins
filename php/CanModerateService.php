<?php
/**
 * CanModerate php plugin
 *
 * PHP version 5
 *
 * LICENSE: This source file is subject to version 3.01 of the PHP license
 * that is available through the world-wide-web at the following URI:
 * http://www.php.net/license/3_01.txt.  If you did not receive a copy of
 * the PHP License and are unable to obtain it through the web, please
 * send a note to license@php.net so we can mail you a copy immediately.
 *
 * @category   Plugin
 * @package    CanModerateService
 * @author     Yevhen Lysyakov <neolysy@gmail.com>
 * @author     Oleh Havrylyuk <o.gavril@gmail.com>
 * @copyright  2014 CanModerate
 * @license    MIT
 * @version    SVN: $Id$
 * @link       http://canmoderate.com
 * @since      File available since Release 0.0.1
 */

class ModerationContent {
	public $lang = null;
    public $serverIp = null;
    public $serverDomain = null;
    public $ip = null;
    public $trackingId = null;
    public $userId = null;
    public $message = '';
    public $messageFormat = null;
};

class CanModerateService {
	const MaxRetryCount = 3;
	const OAuthTokenPrefix = "Bearer";
	const WebHeaderKeyAuthorization = "Authorization";
	const DefaultGrantType = "refresh_token";
	const UrlNewToken = "https://api.canmoderate.com/api/oauth/token";
	const UrlVerifyContent = "https://api.canmoderate.com/api/message/validate";

	private $accessToken = null;
	private $accessTokenRequest = null;
	private $retryCount = 0;

	function __construct($client_id, $client_secret, $refresh_token) {
       $this->setUserCredentials($client_id, $client_secret, $refresh_token);
    }

	public function checkContent($content) {
		try {
			$result = $this->sendRequest($content, self::UrlVerifyContent, $this->getToken());
			$this->retryCount = 0;
		} catch(Exception $e) {
			$errorCode = $e->getCode();

			if ($errorCode == 403 && $this->retryCount + 1 <= self::MaxRetryCount) {
				$this->retryCount++;
				$this->setToken(null);

				$this->checkContent($content);
			} 

			throw $e;
		}

		return $result;
	}

	public function setUserCredentials($client_id, $client_secret, $refresh_token) {
		$this->accessTokenRequest = array(
			'grant_type' => self::DefaultGrantType,
			'client_id' => $client_id,
			'client_secret' => $client_secret,
			'refresh_token' => $refresh_token
		);
	}

	private function getNewToken() {
		$result = json_decode(
			$this->sendRequest($this->accessTokenRequest, self::UrlNewToken)
		);
		return $result->access_token;
	}

	private function getToken() {
		if (!$this->accessToken) {
			$this->setToken($this->getNewToken());
		}
		return $this->accessToken;
	}

	private function setToken($accessToken) {
		$this->accessToken = $accessToken;
	}

	private function sendRequest($content, $src, $accessToken = false) {
		$content = json_encode($content);
		$headers = Array("Content-Type: application/json");

		if( $curl = curl_init() ) {
		    curl_setopt($curl, CURLOPT_URL, $src);
		    curl_setopt($curl, CURLOPT_VERBOSE, true);
		    
		    if ($accessToken) {
		    	array_push($headers, 'Authorization: '.self::OAuthTokenPrefix.' '.$accessToken);
		    }
		    curl_setopt($curl, CURLOPT_HTTPHEADER, $headers);

		    curl_setopt($curl, CURLOPT_RETURNTRANSFER, true);
		    curl_setopt($curl, CURLOPT_POST, true);
		    curl_setopt($curl, CURLOPT_POSTFIELDS, $content);
			curl_setopt($curl, CURLOPT_SSL_VERIFYHOST, 0);
			curl_setopt($curl, CURLOPT_SSL_VERIFYPEER, 0);
		    $output = curl_exec($curl);

			$httpcode = curl_getinfo($curl, CURLINFO_HTTP_CODE);
			if ($httpcode != 200) {
				throw new Exception("http request failed", $httpcode);
			}

		    curl_close($curl);

		    return $output;
		}
	}
}


// example
$service = new CanModerateService('your_client_id', 'your_secret', 'your_access_token');
$content = new ModerationContent();
$content->message = 'My test content string';

$validationResult = json_decode($service->checkContent($content));

print_r($validationResult);


/*
 
// you can also map the result to the following classes

class ValidationResult {
	// policyType
    public $vcType = null;

    public $vcid = null;
    public $listId = null;

    public $listName = null;
    public $entry = null;

    // policyResult
    public $result = null;
    public $resultMessage = null;
}

class ValidationResults {
    public $id = null;
    public $trackingId = null;

    // validationResult collection
    public $results = array();
}

*/



