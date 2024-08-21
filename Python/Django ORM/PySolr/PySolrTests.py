from datetime import datetime

import pysolr
from tqdm import tqdm

from Common.TestResult import TestResult


class PySolrTests:
    def __init__(self):
        self.solrProducts = pysolr.Solr('http://localhost:8983/solr/Products', always_commit=True, timeout = 10)

    def singleRecordSearch(self, samplesQuantity):
        test_result = TestResult(samplesQuantity, "singleRecordSearch")
        for i in tqdm(range(samplesQuantity)):
            now = datetime.now()
            result = self.solrProducts.search('1',fl='id name category price description', qf='id',defType='edismax').docs
            elapsed = now.microsecond // 1000
            test_result.add_sample(elapsed)
        return test_result

