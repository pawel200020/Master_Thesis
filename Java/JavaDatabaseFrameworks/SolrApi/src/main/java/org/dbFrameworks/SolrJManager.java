package org.dbFrameworks;

import me.tongfei.progressbar.ProgressBar;
import org.apache.solr.client.solrj.SolrQuery;
import org.apache.solr.client.solrj.SolrServerException;
import org.apache.solr.client.solrj.impl.HttpSolrClient;
import org.apache.solr.client.solrj.impl.XMLResponseParser;
import org.apache.solr.client.solrj.response.QueryResponse;
import org.dbFrameworks.Managers.IFrameworkManager;
import org.dbFrameworks.entities.Client;
import org.dbFrameworks.entities.Product;

import java.io.*;
import java.nio.charset.StandardCharsets;
import java.time.Duration;
import java.time.Instant;
import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.ThreadLocalRandom;

public class SolrJManager implements IFrameworkManager {
    HttpSolrClient _solrClients;
    HttpSolrClient _solrProducts;

    public SolrJManager() {
        _solrClients = new HttpSolrClient.Builder("http://localhost:8983/solr/Clients").build();
        _solrClients.setParser(new XMLResponseParser());

        _solrProducts = new HttpSolrClient.Builder("http://localhost:8983/solr/Products").build();
        _solrProducts.setParser(new XMLResponseParser());
    }

    @Override
    public TestResult SingleRecordSearch(int samplesQuantity) {
        var testResult = new TestResult(samplesQuantity, "SingleRecordSearch");
        var productsCount = (int) GetProductsCount();
        try (ProgressBar pb = new ProgressBar("SingleRecordSearch", samplesQuantity)) {
            for (var i = 0; i < samplesQuantity; i++) {
                var idToSearch = ThreadLocalRandom.current().nextInt(0, productsCount);
                Instant start = Instant.now();
                SolrQuery q = new SolrQuery();
                q.set("defType", "edismax");
                q.addField("id");
                q.addField("name");
                q.addField("category");
                q.addField("price");
                q.addField("description");
                q.set("qf", "id");
                q.setQuery(String.valueOf(idToSearch));

                try {

                    QueryResponse response = _solrProducts.query(q);
                    var list = response.getBeans(Product.class);
                    Instant finish = Instant.now();
                    testResult.AddMeasure(Duration.between(start, finish).toMillis());
                    pb.step();
                } catch (SolrServerException | IOException e) {
                }
            }
        }
        return testResult;
    }

    @Override
    public TestResult SetOfDataSearch(int samplesQuantity) {
        var testResult = new TestResult(samplesQuantity, "SetOfDataSearch");
        var names = getFileFromResources("first_words.txt");
        try (ProgressBar pb = new ProgressBar("SetOfDataSearch", samplesQuantity)) {
            for (var i = 0; i < samplesQuantity; i++) {
                var nameToSearch = names.get(ThreadLocalRandom.current().nextInt(0, names.size()));
                Instant start = Instant.now();
                SolrQuery q = new SolrQuery();
                q.set("defType", "edismax");
                q.addField("id");
                q.addField("name");
                q.addField("age");
                q.addField("salary");
                q.addField("description");
                q.addField("favourite_product");
                q.addField("recent_bought_products");
                q.addField("birth_date");
                q.addField("has_children_b");
                q.addField("how_many_cats_i");
                q.addField("has_partner_b");
                q.set("qf", "name");
                q.setRows(10000);
                q.setQuery(nameToSearch);
                try {
                    QueryResponse response = _solrClients.query(q);
                    var list = response.getBeans(Client.class);
                    Instant finish = Instant.now();
                    testResult.AddMeasure(Duration.between(start, finish).toMillis());
                    pb.step();
                } catch (SolrServerException | IOException e) {
                }
            }
        }
        return testResult;
    }

    @Override
    public TestResult SetOfDataWithIsNullSearch(int samplesQuantity) {
        var testResult = new TestResult(samplesQuantity, "SetOfDataWithIsNullSearch");
        try (ProgressBar pb = new ProgressBar("SetOfDataWithIsNullSearch", samplesQuantity)) {
            for (var i = 0; i < samplesQuantity; i++) {
                var field = i % 3 == 0
                        ? "has_children_b"
                        : i % 3 == 1
                        ? "how_many_cats_i"
                        : "has_partner_b";
                Instant start = Instant.now();
                SolrQuery q = new SolrQuery();
                q.set("defType", "edismax");
                q.addField("id");
                q.addField("name");
                q.addField("age");
                q.addField("salary");
                q.addField("description");
                q.addField("favourite_product");
                q.addField("recent_bought_products");
                q.addField("birth_date");
                q.addField("has_children_b");
                q.addField("how_many_cats_i");
                q.addField("has_partner_b");
                q.set("qf", "name");
                q.setQuery("NOT " + field + ":*");
                q.setRows(10000);

                try {
                    QueryResponse response = _solrClients.query(q);
                    var list = response.getBeans(Client.class);
                    Instant finish = Instant.now();
                    testResult.AddMeasure(Duration.between(start, finish).toMillis());
                    pb.step();
                } catch (SolrServerException | IOException e) {
                }

            }
        }
        return testResult;
    }

    @Override
    public TestResult AddRecords(int samplesQuantity) {
        var testResult = new TestResult(samplesQuantity, "AddRecords");
        var categories = getFileFromResources("categories.txt");
        var productsCount = GetProductsCount();
        var idsToRemove = new ArrayList<String>();
        try (ProgressBar pb = new ProgressBar("AddRecords", samplesQuantity)) {
            for (var i = 0; i < samplesQuantity; i++) {
                var category = categories.get(ThreadLocalRandom.current().nextInt(0, categories.size()));
                var price = (double) (ThreadLocalRandom.current().nextInt(0, 9999999)) / 100;
                var id = String.valueOf(productsCount + i + 1);
                var name = "Example product " + i;
                Instant start = Instant.now();
                var product = new Product(id, name, "added by test", price, category);
                try {
                    _solrProducts.addBean(product);
                    _solrProducts.commit();
                } catch (SolrServerException | IOException e) {
                }
                Instant finish = Instant.now();
                testResult.AddMeasure(Duration.between(start, finish).toMillis());
                pb.step();
                idsToRemove.add(id);
            }
        }
        RemoveRecordsSilently(idsToRemove);
        return testResult;
    }

    @Override
    public TestResult EditRecords(int samplesQuantity) {
        var ids = AddRecordsSilently(samplesQuantity);
        var testResult = new TestResult(samplesQuantity, "EditRecords");
        try (ProgressBar pb = new ProgressBar("EditRecords", samplesQuantity)) {
            for (var i = 0; i < samplesQuantity; i++) {
                var first = Integer.valueOf(ids.get(0));
                var idToSearch = ThreadLocalRandom.current().nextInt(first, first + ids.size());
                Instant start = Instant.now();
                SolrQuery q = new SolrQuery();
                q.set("defType", "edismax");
                q.addField("id");
                q.addField("name");
                q.addField("category");
                q.addField("price");
                q.addField("description");
                q.set("qf", "id");
                q.setQuery(String.valueOf(idToSearch));

                try {
                    QueryResponse response = _solrProducts.query(q);
                    var product = response.getBeans(Product.class).get(0);
                    product.description = "Edited value set for this by iteration " + i;

                    _solrProducts.addBean(product);
                    _solrProducts.commit();
                } catch (SolrServerException | IOException e) {
                }
                Instant finish = Instant.now();
                testResult.AddMeasure(Duration.between(start, finish).toMillis());
                pb.step();
            }
        }
        RemoveRecordsSilently(ids);
        return testResult;
    }

    @Override
    public TestResult DeleteRecords(int samplesQuantity) {
        var ids = AddRecordsSilently(samplesQuantity);
        var testResult = new TestResult(samplesQuantity, "DeleteRecords");
        try (ProgressBar pb = new ProgressBar("DeleteRecords", ids.size())) {
            for (var id : ids) {
                try {
                    Instant start = Instant.now();
                    _solrProducts.deleteById(id);
                    _solrProducts.commit();
                    Instant finish = Instant.now();
                    testResult.AddMeasure(Duration.between(start, finish).toMillis());

                } catch (SolrServerException | IOException ignored) {
                }

                pb.step();
            }
        }
        return testResult;
    }

    private long GetProductsCount() {
        SolrQuery q = new SolrQuery();
        q.addField("id");
        q.setQuery("*:*");
        try {
            return (_solrProducts.query(q)).getResults().getNumFound();
        } catch (SolrServerException | IOException e) {
        }
        return -1;
    }


    private List<String> getFileFromResources(String fileName) {
        List<String> productNames = new ArrayList<>();
        try {
            ClassLoader classloader = Thread.currentThread().getContextClassLoader();
            InputStream inputStream = classloader.getResourceAsStream(fileName);
            if (inputStream == null) {
                throw new FileNotFoundException(fileName + " not found");
            }
            InputStreamReader streamReader = new InputStreamReader(inputStream, StandardCharsets.UTF_8);
            BufferedReader reader = new BufferedReader(streamReader);
            for (String line; (line = reader.readLine()) != null; ) {
                productNames.add(line);
            }
            reader.close();
        } catch (FileNotFoundException e) {
            System.out.println("An error occurred.");
        } catch (IOException e) {
            throw new RuntimeException(e);
        }
        return productNames;
    }

    private void RemoveRecordsSilently(List<String> itemsToRemove) {
        try (ProgressBar pb = new ProgressBar("RemoveRecordsSilently", itemsToRemove.size())) {
            for (var id : itemsToRemove) {
                try {
                    _solrProducts.deleteById(id);
                } catch (SolrServerException | IOException ignored) {
                }
                pb.step();
            }
        }
        try {
            _solrProducts.commit();
        } catch (SolrServerException | IOException ignored) {
        }
    }

    private List<String> AddRecordsSilently(int samplesQuantity) {
        var productsCount = GetProductsCount();
        var categories = getFileFromResources("categories.txt");
        var idsToRemove = new ArrayList<String>();
        try (ProgressBar pb = new ProgressBar("AddRecordsSilently", samplesQuantity)) {
            for (var i = 0; i < samplesQuantity; i++) {
                var category = categories.get(ThreadLocalRandom.current().nextInt(0, categories.size()));
                var price = (double) (ThreadLocalRandom.current().nextInt(0, 9999999)) / 100;
                var id = String.valueOf(productsCount + i + 1);
                var name = "Example product " + i;
                var product = new Product(id, name, "added by test", price, category);
                try {
                    _solrProducts.addBean(product);
                    _solrProducts.commit();
                } catch (SolrServerException | IOException e) {
                }
                idsToRemove.add(id);
                pb.step();
            }
        }
        try {
            _solrProducts.commit();
        } catch (SolrServerException | IOException e) {
        }
        return idsToRemove;
    }
}
