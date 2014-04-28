package com.canmoderate.client;

import com.canmoderate.client.data.*;
import com.canmoderate.client.data.VersionData;
import com.fasterxml.jackson.databind.ObjectMapper;

import javax.net.ssl.*;
import javax.security.cert.CertificateException;
import javax.security.cert.X509Certificate;
import java.io.*;
import java.net.HttpURLConnection;
import java.net.URL;
import java.security.KeyManagementException;
import java.security.NoSuchAlgorithmException;
import java.security.SecureRandom;
import java.util.logging.Level;
import java.util.logging.Logger;

/**
 * Created by alyas on 4/25/14.
 */
public final  class CanModerateService {


    public static String getJSON(URL url) {
        return sendHttpRequest(url,  "GET", null);
    }

    public static String postJSON(URL url, String data) {
        return sendHttpRequest(url, "POST", data);
    }
    public static String postJSON(URL url, String data, String token ) {
        return sendHttpRequest(url, "POST", data, token);
    }

    private static void setSSL() throws KeyManagementException, NoSuchAlgorithmException {
        SSLContext ctx = SSLContext.getInstance("TLS");
        ctx.init(new KeyManager[0], new TrustManager[] {new DefaultTrustManager()}, new SecureRandom());
        SSLContext.setDefault(ctx);
    }

    private static String sendHttpRequest(URL url, String method, String data) {
        return sendHttpRequest(url, method, data, null);
    }

    private static String sendHttpRequest(URL url, String method, String data, String token) {
        try {
            int timeout = 5000;
            HttpsURLConnection connection = (HttpsURLConnection) url.openConnection();
            connection.setHostnameVerifier(new HostnameVerifier() {
                @Override
                public boolean verify(String arg0, SSLSession arg1) {
                    return true;
                }
            });
            connection.setRequestMethod(method);
            if(data!=null)
                connection.setRequestProperty("Content-length", Integer.toString(data.length()));
            else
                connection.setRequestProperty("Content-length", "0");

            connection.setRequestProperty("Content-Type", "application/json" );
            connection.setUseCaches(false);

            connection.setAllowUserInteraction(false);
            connection.setConnectTimeout(timeout);
            connection.setReadTimeout(timeout);

            if(token!=null) {
                connection.setRequestProperty("Authorization", "Bearer "+token );
            }

            if(method=="POST") {
                connection.setDoInput(true);
                connection.setDoOutput(true);
                OutputStream os = connection.getOutputStream();
                BufferedWriter writer = new BufferedWriter(
                        new OutputStreamWriter(os, "UTF-8"));

                writer.write(data);
                writer.flush();
                writer.close();
                os.close();
            }
            connection.connect();

            int status = connection.getResponseCode();

            switch (status) {
                case 200:
                case 201:
                    BufferedReader br = new BufferedReader(new InputStreamReader(connection.getInputStream()));
                    StringBuilder sb = new StringBuilder();
                    String line;
                    while ((line = br.readLine()) != null) {
                        sb.append(line+"\n");
                    }
                    br.close();
                    return sb.toString();
                case 401:
                    Logger.getLogger("").log(Level.SEVERE, "401 response from "+url.toString());
            }

        } catch (Exception ex) {
            Logger.getLogger("").log(Level.SEVERE, null, ex);
        }
        return null;
    }

    private static  String httpGet(URL url) throws Exception {
        setSSL();
        return getJSON(url);
    }
    private static  String httpPost(URL url, String data) throws Exception {
        setSSL();
        return postJSON(url, data);
    }

    private static String httpPost(URL url, String data, String token) throws Exception {
        setSSL();
        return postJSON(url, data, token);
    }

    public static RefreshTokenResponse refreshToken(RefreshTokenRequest data) throws Exception {

        URL url = new URL("  https://api.canmoderate.com/api/oauth/token");
        ObjectMapper mapper = new ObjectMapper();
        String stringRequest  = mapper.writeValueAsString(data);
        String jsonResponseString = httpPost(url, stringRequest);
        RefreshTokenResponse response = mapper.readValue(jsonResponseString, RefreshTokenResponse.class);
        return response;
    }

    public static VersionData getVersion() throws Exception {

        URL url = new URL("https://api.canmoderate.com/api/version");
        String jsonResponseString = httpGet(url);

        ObjectMapper mapper = new ObjectMapper();
        VersionResponse version= mapper.readValue(jsonResponseString, VersionResponse.class);
        return version.data;
    }

    public static MessageResponse validateMessage(String token, MessageRequest message) throws Exception {
        URL url = new URL("https://api.canmoderate.com/api/message/validate");
        ObjectMapper mapper = new ObjectMapper();

        String stringRequest  = mapper.writeValueAsString(message);
        String jsonResponseString = httpPost(url, stringRequest, token);

        MessageResponse response = mapper.readValue(jsonResponseString, MessageResponse.class);
        return response;
    }
}
