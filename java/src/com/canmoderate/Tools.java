package com.canmoderate;


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
            SSLContext ctx = SSLContext.getInstance("TLS");
            ctx.init(new KeyManager[0], new TrustManager[] {new DefaultTrustManager()}, new SecureRandom());
            SSLContext.setDefault(ctx);

            URL url = new URL("https://api.canmoderate.com/api/version");
            HttpsURLConnection conn = (HttpsURLConnection) url.openConnection();

            conn.setHostnameVerifier(new HostnameVerifier() {
                @Override
                public boolean verify(String arg0, SSLSession arg1) {
                    return true;
                }
            });

            System.out.println(conn.getResponseCode());
            conn.disconnect();

            System.out.println(getJSON(url, 10000));
            /*SSLContext ctx = SSLContext.getInstance("TLS");
            ctx.init(new KeyManager[0], new TrustManager[] {new DefaultTrustManager()}, new SecureRandom());
            SSLContext.setDefault(ctx);
            URL url = new URL("https://api.canmoderate.com/api/version");
            HttpsURLConnection conn = (HttpsURLConnection) url.openConnection();
            conn.setHostnameVerifier(new HostnameVerifier() {
                @Override
                public boolean verify(String arg0, SSLSession arg1) {
                    return true;
                }
            });
            System.out.println(conn.getResponseCode());
            System.out.println(conn.getContent().toString());
            conn.disconnect();

            HostnameVerifier hostnameVerifier = org.apache.http.conn.ssl.SSLSocketFactory.ALLOW_ALL_HOSTNAME_VERIFIER;

            SSLContextBuilder builder = new SSLContextBuilder();
            builder.loadTrustMaterial(null, new TrustApiSignedStrategy());
            SSLConnectionSocketFactory sslsf = new SSLConnectionSocketFactory(
                    builder.build());
            CloseableHttpClient httpclient = HttpClients.custom().setSSLSocketFactory(
                    sslsf).setHostnameVerifier(new BrowserCompatHostnameVerifier()).
                    build();

            HttpGet httpGet = new HttpGet("https://api.canmoderate.com/api/version");
            CloseableHttpResponse response = httpclient.execute(httpGet);
            try {
                System.out.println("Status:");
                System.out.println(response.getStatusLine());
                HttpEntity entity = response.getEntity();
                EntityUtils.consume(entity);
            }
            finally {
                response.close();
            }
*/

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

