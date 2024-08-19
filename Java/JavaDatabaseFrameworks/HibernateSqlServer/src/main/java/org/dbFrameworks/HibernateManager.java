package org.dbFrameworks;

import jakarta.persistence.EntityManager;
import jakarta.persistence.EntityManagerFactory;
import me.tongfei.progressbar.ProgressBar;
import org.dbFrameworks.Managers.IRelationalFrameworkManager;
import org.dbFrameworks.entities.*;
import org.dbFrameworks.persistence.CustomPersistenceUnitInfo;
import org.hibernate.jpa.HibernatePersistenceProvider;

import java.io.*;
import java.nio.charset.StandardCharsets;
import java.sql.Date;
import java.time.Duration;
import java.time.Instant;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.HashSet;
import java.util.List;
import java.util.concurrent.ThreadLocalRandom;

public class HibernateManager implements IRelationalFrameworkManager {


    EntityManagerFactory contextFactory;

    public HibernateManager() {
        contextFactory = new HibernatePersistenceProvider().createContainerEntityManagerFactory(new CustomPersistenceUnitInfo(), new HashMap<>());
    }

    @Override
    public TestResult SingleRecordSearch(int samplesQuantity) {
        var testResult = new TestResult(samplesQuantity, "SingleRecordSearch");
        EntityManager em = contextFactory.createEntityManager();
        try (ProgressBar pb = new ProgressBar("SingleRecordSearch", samplesQuantity)) {
            for (var i = 0; i < samplesQuantity; i++) {
                int index = ThreadLocalRandom.current().nextInt(1, 40000);
                Instant start = Instant.now();
                em.getTransaction().begin();
                var item = em.find(Orders.class, index);
                em.getTransaction().commit();
                Instant finish = Instant.now();
                testResult.AddMeasure(Duration.between(start, finish).toMillis());
                pb.step();
            }
        }
        em.close();
        return testResult;
    }

    @Override
    public TestResult SetOfDataSearch(int samplesQuantity) {
        var testResult = new TestResult(samplesQuantity, "SetOfDataSearch");
        EntityManager em = contextFactory.createEntityManager();

        int positionsCount = getPositionsCount(em);

        try (ProgressBar pb = new ProgressBar("SetOfDataSearch", samplesQuantity)) {
            for (var i = 0; i < samplesQuantity; i++) {
                int positionToSearch = ThreadLocalRandom.current().nextInt(1, positionsCount);
                Instant start = Instant.now();
                em.getTransaction().begin();
                var q = em.createQuery("Select e FROM Employees e WHERE e.Position.id=:position", Employees.class);
                q.setParameter("position", positionToSearch);
                var result = q.getResultList().toArray();
                em.getTransaction().commit();
                Instant finish = Instant.now();
                testResult.AddMeasure(Duration.between(start, finish).toMillis());
                pb.step();
            }
        }
        em.close();
        return testResult;
    }

    @Override
    public TestResult SetOfDataWithIsNullSearch(int samplesQuantity) {
        var testResult = new TestResult(samplesQuantity, "SingleRecordSearch");
        EntityManager em = contextFactory.createEntityManager();
        var tablesWithNullQuantity = TableWithNull.values().length;
        try (ProgressBar pb = new ProgressBar("SingleRecordSearch", samplesQuantity)) {
            for (var i = 0; i < samplesQuantity; i++) {
                var table = TableWithNull.values()[ThreadLocalRandom.current().nextInt(1, tablesWithNullQuantity)];
                switch (table) {
                    case ClientPhone -> {
                        Instant start = Instant.now();
                        em.getTransaction().begin();
                        var q = em.createQuery("Select e FROM Clients e WHERE e.PhoneNumber is null ", Clients.class);
                        var result = q.getResultList().toArray();
                        em.getTransaction().commit();
                        Instant finish = Instant.now();
                        testResult.AddMeasure(Duration.between(start, finish).toMillis());
                        break;
                    }
                    case ClientCountry -> {
                        Instant start = Instant.now();
                        em.getTransaction().begin();
                        var q = em.createQuery("Select e FROM Clients e WHERE e.Country is null ", Clients.class);
                        var result = q.getResultList().toArray();
                        em.getTransaction().commit();
                        Instant finish = Instant.now();
                        testResult.AddMeasure(Duration.between(start, finish).toMillis());
                        break;
                    }
                    case EmployeesPhone -> {
                        Instant start = Instant.now();
                        em.getTransaction().begin();
                        var q = em.createQuery("Select e FROM Employees e WHERE e.PhoneNumber is null ", Employees.class);
                        var result = q.getResultList().toArray();
                        em.getTransaction().commit();
                        Instant finish = Instant.now();
                        testResult.AddMeasure(Duration.between(start, finish).toMillis());
                        break;
                    }
                    case EmployeesPositionId -> {
                        Instant start = Instant.now();
                        em.getTransaction().begin();
                        var q = em.createQuery("Select e FROM Employees e WHERE e.Position is null ", Employees.class);
                        var result = q.getResultList().toArray();
                        em.getTransaction().commit();
                        Instant finish = Instant.now();
                        testResult.AddMeasure(Duration.between(start, finish).toMillis());
                        break;
                    }
                    case Orders -> {
                        Instant start = Instant.now();
                        em.getTransaction().begin();
                        var q = em.createQuery("Select e FROM Orders e WHERE e.OrderDate is null ", Orders.class);
                        var result = q.getResultList().toArray();
                        em.getTransaction().commit();
                        Instant finish = Instant.now();
                        testResult.AddMeasure(Duration.between(start, finish).toMillis());
                        break;
                    }
                    case Products -> {
                        Instant start = Instant.now();
                        em.getTransaction().begin();
                        var q = em.createQuery("Select e FROM Products e WHERE e.ProductDescription is null ", Products.class);
                        var result = q.getResultList().toArray();
                        em.getTransaction().commit();
                        Instant finish = Instant.now();
                        testResult.AddMeasure(Duration.between(start, finish).toMillis());
                        break;
                    }
                }
                pb.step();
            }
        }
        em.close();
        return testResult;
    }

    @Override
    public TestResult AddRecords(int samplesQuantity) {
        var testResult = new TestResult(samplesQuantity, "AddRecords");
        var names = getFileFromResources("products.txt");
        List<Integer> idsToRemove = new ArrayList<>();
        EntityManager em = contextFactory.createEntityManager();
        try (ProgressBar pb = new ProgressBar("AddRecords", samplesQuantity)) {
            for (var i = 0; i < samplesQuantity; i++) {
                var name = names.get(ThreadLocalRandom.current().nextInt(0, names.size()));
                Instant start = Instant.now();
                em.getTransaction().begin();
                Products p = new Products();
                p.setProductName(name);
                p.setPrice(21.37);
                p.setProductDescription("AddedByTest");
                p.setSupplier("None");
                em.persist(p);
                var id = p.getProductId();
                em.getTransaction().commit();
                Instant finish = Instant.now();
                testResult.AddMeasure(Duration.between(start, finish).toMillis());
                idsToRemove.add(id);
                pb.step();
            }
        }
        em.close();
        removeRecordsSilent(idsToRemove);
        return testResult;
    }

    @Override
    public TestResult EditRecords(int samplesQuantity) {
        var addedIds = addRecordsSilent(samplesQuantity, getFileFromResources("products.txt"));
        var testResult = new TestResult(samplesQuantity, "EditRecords");
        var names = getFileFromResources("products.txt");
        EntityManager em = contextFactory.createEntityManager();
        try (ProgressBar pb = new ProgressBar("EditRecords", samplesQuantity)) {
            for (var i = 0; i < samplesQuantity; i++) {
                var recordId = addedIds.get(ThreadLocalRandom.current().nextInt(0, addedIds.size()));
                var name = names.get(ThreadLocalRandom.current().nextInt(0, names.size()));
                Instant start = Instant.now();
                em.getTransaction().begin();
                var product = em.find(Products.class, recordId);
                product.setProductName(name);
                em.persist(product);
                em.getTransaction().commit();
                Instant finish = Instant.now();
                testResult.AddMeasure(Duration.between(start, finish).toMillis());
                pb.step();
            }
        }
        em.close();
        removeRecordsSilent(addedIds);
        return testResult;
    }

    @Override
    public TestResult DeleteRecords(int samplesQuantity) {
        var addedIds = addRecordsSilent(samplesQuantity, getFileFromResources("products.txt"));
        var testResult = new TestResult(samplesQuantity, "DeleteRecords");
        EntityManager em = contextFactory.createEntityManager();
        try (ProgressBar pb = new ProgressBar("DeleteRecords", samplesQuantity)) {
            for (var id : addedIds) {
                Instant start = Instant.now();
                em.getTransaction().begin();
                var product = em.find(Products.class, id);
                em.remove(product);
                em.getTransaction().commit();
                Instant finish = Instant.now();
                testResult.AddMeasure(Duration.between(start, finish).toMillis());
                pb.step();
            }
        }
        em.close();
        return testResult;
    }

    @Override
    public TestResult SearchTwoRelatedTables(int samplesQuantity) {
        var testResult = new TestResult(samplesQuantity, "SearchTwoRelatedTables");
        EntityManager em = contextFactory.createEntityManager();

        int positionsCount = getPositionsCount(em);

        try (ProgressBar pb = new ProgressBar("SearchTwoRelatedTables", samplesQuantity)) {
            for (var i = 0; i < samplesQuantity; i++) {
                int positionToSearch = ThreadLocalRandom.current().nextInt(1, positionsCount);
                em.getTransaction().begin();
                var position = em.find(Positions.class, positionToSearch);
                em.getTransaction().commit();
                Instant start = Instant.now();
                em.getTransaction().begin();
                var q = em.createQuery("Select e FROM Employees e WHERE e.Position.PositionName = :positionName", Employees.class);
                q.setParameter("positionName", position.getPositionName());
                var result = q.getResultList().toArray();
                ;
                em.getTransaction().commit();
                Instant finish = Instant.now();
                testResult.AddMeasure(Duration.between(start, finish).toMillis());
                pb.step();
            }
        }
        em.close();
        return testResult;
    }

    @Override
    public TestResult SearchFourRelatedTables(int samplesQuantity) {
        var testResult = new TestResult(samplesQuantity, "SearchFourRelatedTables");
        EntityManager em = contextFactory.createEntityManager();

        int positionsCount = getPositionsCount(em);
        int storesCount = getStoresCount(em);

        try (ProgressBar pb = new ProgressBar("SearchFourRelatedTables", samplesQuantity)) {
            for (var i = 0; i < samplesQuantity; i++) {
                em.getTransaction().begin();
                var position = em.find(Positions.class, ThreadLocalRandom.current().nextInt(1, positionsCount));
                var store = em.find(Stores.class, ThreadLocalRandom.current().nextInt(1, storesCount));
                em.getTransaction().commit();
                Instant start = Instant.now();
                em.getTransaction().begin();
                var q = em.createQuery("SELECT o FROM Orders o WHERE o.employee.Position.PositionName = :positionToFind AND o.store.Country = :storeCountry", Employees.class);
                q.setParameter("positionToFind", position.getPositionName());
                q.setParameter("storeCountry", store.getCountry());
                var result = q.getResultList().toArray();
                em.getTransaction().commit();
                Instant finish = Instant.now();
                testResult.AddMeasure(Duration.between(start, finish).toMillis());
                pb.step();
            }
        }
        em.close();
        return testResult;
    }

    @Override
    public TestResult SearchRecordsWhichDoesNotHaveConnection(int samplesQuantity) {
        var testResult = new TestResult(samplesQuantity, "SearchRecordsWhichDoesNotHaveConnection");
        EntityManager em = contextFactory.createEntityManager();
        try (ProgressBar pb = new ProgressBar("SearchRecordsWhichDoesNotHaveConnection", samplesQuantity)) {
            for (var i = 0; i < samplesQuantity; i++) {
                //if (i % 2 != 0) {
                Instant start = Instant.now();
                em.getTransaction().begin();
                var q = em.createQuery("SELECT p FROM Employees p WHERE p.Position is not null", Employees.class);
                var result = q.getResultList();
                List<String> usedPositions = new ArrayList<String>(new HashSet<>(result.stream()
                        .map(object -> object.getPosition().getPositionName())
                        .toList()));
                var q2 = em.createQuery("SELECT p FROM Positions p WHERE p.PositionName not in :arr", Positions.class);
                q2.setParameter("arr", usedPositions);
                var result2 = q2.getResultList();
                em.getTransaction().commit();
                Instant finish = Instant.now();
                testResult.AddMeasure(Duration.between(start, finish).toMillis());
                pb.step();
//                } else {
//                    Instant start = Instant.now();
//                    em.getTransaction().begin();
//                    var q = em.createQuery("SELECT p FROM Stores p WHERE p.Orders.size > 0", Stores.class);
//                    var result = q.getResultList();
//                    em.getTransaction().commit();
//                    Instant finish = Instant.now();
                testResult.AddMeasure(Duration.between(start, finish).toMillis());
//                    pb.step();
//                }
            }
        }
        em.close();
        return testResult;
    }

    @Override
    public TestResult SearchWithSubQuery(int samplesQuantity) {
        var testResult = new TestResult(samplesQuantity, "SearchWithSubQuery");
        var countries = getFileFromResources("Country.txt");
        EntityManager em = contextFactory.createEntityManager();
        try (ProgressBar pb = new ProgressBar("SearchWithSubQuery", samplesQuantity)) {
            for (var i = 0; i < samplesQuantity; i++) {
                var countryToFind = countries.get(ThreadLocalRandom.current().nextInt(0, countries.size()));
                Instant start = Instant.now();
                em.getTransaction().begin();
                var q = em.createQuery("SELECT o FROM Orders o WHERE o.client.Country =:ctf", Orders.class);
                q.setParameter("ctf", countryToFind);
                var result = q.getResultList().toArray();
                em.getTransaction().commit();
                Instant finish = Instant.now();
                testResult.AddMeasure(Duration.between(start, finish).toMillis());
                pb.step();
            }
        }
        em.close();
        return testResult;
    }

    @Override
    public TestResult RemoveRelatedRecords(int samplesQuantity) {
        var testResult = new TestResult(samplesQuantity, "RemoveRelatedRecords");
        EntityManager em = contextFactory.createEntityManager();
        var clientCount = getClientsCount(em);
        var employeeCount = getEmployeeCount(em);
        try (ProgressBar pb = new ProgressBar("RemoveRelatedRecords", samplesQuantity)) {
            for (var i = 0; i < samplesQuantity; i++) {
                em.getTransaction().begin();
                var client = em.find(Clients.class, ThreadLocalRandom.current().nextInt(1, clientCount));
                var employee = em.find(Employees.class, ThreadLocalRandom.current().nextInt(1, employeeCount));
                em.getTransaction().commit();

                em.getTransaction().begin();
                Stores s = new Stores();
                s.setAddress("Norymberska 1 30-412 Krakow");
                s.setCountry("Poland");
                Orders o = new Orders();
                o.setClient(client);
                o.setEmployee(employee);
                o.setOrderDate(new Date(2023, 3, 4));
                o.setOrderDetails(OrderXml);
                o.setTotalCost(120.00);
                o.setStore(s);
                em.persist(s);
                em.persist(o);
                em.getTransaction().commit();

                Instant start = Instant.now();
                em.getTransaction().begin();
                em.remove(s);
                em.getTransaction().commit();

                Instant finish = Instant.now();
                testResult.AddMeasure(Duration.between(start, finish).toMillis());
                pb.step();
            }
        }
        em.close();
        return testResult;
    }

    private void removeRecordsSilent(List<Integer> idsToRemove) {
        EntityManager em = contextFactory.createEntityManager();
        try (ProgressBar pb = new ProgressBar("removeRecordsSilent", idsToRemove.size())) {
            for (var id : idsToRemove) {
                em.getTransaction().begin();
                var product = em.find(Products.class, id);
                em.remove(product);
                em.getTransaction().commit();
                pb.step();
            }
        }
        em.close();
    }

    private List<Integer> addRecordsSilent(int samplesQuantity, List<String> names) {
        EntityManager em = contextFactory.createEntityManager();
        List<Integer> idsToRemove = new ArrayList<>();
        try (ProgressBar pb = new ProgressBar("addRecordsSilent", samplesQuantity)) {
            for (int i = 0; i < samplesQuantity; i++) {
                var name = names.get(ThreadLocalRandom.current().nextInt(0, names.size()));
                em.getTransaction().begin();
                Products p = new Products();
                p.setProductName(name);
                p.setPrice(21.37);
                p.setProductDescription("AddedByTest");
                p.setSupplier("None");
                em.persist(p);
                var id = p.getProductId();
                em.getTransaction().commit();
                idsToRemove.add(id);
                pb.step();
            }
        }
        em.close();
        return idsToRemove;
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

    private int getPositionsCount(EntityManager em) {
        em.getTransaction().begin();
        var posQuantitiyQuery = em.createQuery("Select p From Positions p");
        int positionsCount = posQuantitiyQuery.getResultList().size();
        em.getTransaction().commit();
        return positionsCount;
    }

    private int getClientsCount(EntityManager em) {
        em.getTransaction().begin();
        var posQuantitiyQuery = em.createQuery("Select c From Clients c");
        int positionsCount = posQuantitiyQuery.getResultList().size();
        em.getTransaction().commit();
        return positionsCount;
    }

    private int getEmployeeCount(EntityManager em) {
        em.getTransaction().begin();
        var posQuantitiyQuery = em.createQuery("Select e From Employees e");
        int positionsCount = posQuantitiyQuery.getResultList().size();
        em.getTransaction().commit();
        return positionsCount;
    }

    private int getStoresCount(EntityManager em) {
        em.getTransaction().begin();
        var storesQuantitiyQuery = em.createQuery("Select p From Stores p");
        int storesCount = storesQuantitiyQuery.getResultList().size();
        em.getTransaction().commit();
        return storesCount;
    }

    private String OrderXml = """
            <Order xmlns="urn:OrdersInfoNamespace">
              <Product id="0">
                <Name>rock</Name>
                <Quantity>669</Quantity>
              </Product>
              <Product id="1">
                <Name>toilet</Name>
                <Quantity>837</Quantity>
              </Product>
              <Product id="2">
                <Name>newspaper</Name>
                <Quantity>38</Quantity>
              </Product>
            </Order>""";
}
