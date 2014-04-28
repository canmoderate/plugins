package com.canmoderate;


import com.canmoderate.client.CanModerateService;
import com.canmoderate.client.data.*;
import com.fasterxml.jackson.core.JsonParser;
import com.fasterxml.jackson.databind.ObjectMapper;

import javax.net.ssl.*;
import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;
import java.security.SecureRandom;
import java.security.cert.CertificateException;
import java.security.cert.X509Certificate;
import java.util.UUID;
import java.util.logging.Level;
import java.util.logging.Logger;

public class Tools {

    public static String getJSON(URL url, int timeout) {
        try {

            HttpURLConnection c = (HttpURLConnection) url.openConnection();

            c.setRequestMethod("GET");
            c.setRequestProperty("Content-length", "0");
            c.setUseCaches(false);
            c.setAllowUserInteraction(false);
            c.setConnectTimeout(timeout);
            c.setReadTimeout(timeout);
            c.connect();
            int status = c.getResponseCode();

            switch (status) {
                case 200:
                case 201:
                    BufferedReader br = new BufferedReader(new InputStreamReader(c.getInputStream()));
                    StringBuilder sb = new StringBuilder();
                    String line;
                    while ((line = br.readLine()) != null) {
                        sb.append(line+"\n");
                    }
                    br.close();
                    return sb.toString();
            }

        } catch (Exception ex) {
            Logger.getLogger("").log(Level.SEVERE, null, ex);
        }
        return null;
    }


    public static void main(String[] args) {
        System.out.println("CanModerate Client Tools 1.0");

        try {

            System.out.println("version: "+CanModerateService.getVersion().version);
            System.out.println("server date: "+CanModerateService.getVersion().date);
            System.out.println("--------");
            System.out.println("Open connection with Oauth keys");
            RefreshTokenRequest refreshTokenRequest = new RefreshTokenRequest();
            //fill outh parameters
            refreshTokenRequest.grant_type = "refresh_token";
            refreshTokenRequest.client_id = "BBRALWUMGRRTNZIEQRWOQFET";
            refreshTokenRequest.client_secret = "432526176353";
            refreshTokenRequest.refresh_token = "84d0a86d-2354-416f-a249-e5451848b643";
            RefreshTokenResponse refreshTokenResponse  = CanModerateService.refreshToken(refreshTokenRequest);

            System.out.println("Access token: "+refreshTokenResponse.access_token);

            MessageRequest message = new MessageRequest();
            message.clientIp = "10.0.0.1";
            message.trackingId = UUID.randomUUID().toString();

            message.message = "test message";

            MessageResponse messageResponse = CanModerateService.validateMessage(refreshTokenResponse.access_token, message);
            System.out.println("Response trackingId: "+messageResponse.trackingId);
            for (MessageResult result : messageResponse.results) {
                System.out.println("Result: "+result.result + " type: "+result.vcType);
            }



        } catch(Exception e) {
            System.out.print("Error: ");
            System.out.println(e.getMessage());
        }
    }

    private static class DefaultTrustManager implements X509TrustManager {

        @Override
        public void checkClientTrusted(X509Certificate[] arg0, String arg1) throws CertificateException {}

        @Override
        public void checkServerTrusted(X509Certificate[] arg0, String arg1) throws CertificateException {}

        @Override
        public X509Certificate[] getAcceptedIssuers() {
            return null;
        }
    }
}

