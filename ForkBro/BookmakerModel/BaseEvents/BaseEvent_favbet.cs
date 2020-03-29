using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForkBro.BookmakerModel.BaseEvents
{
    public class Outcome
    {

        [JsonProperty("outcome_coef")]
        public double OutcomeCoef { get; set; }

        [JsonProperty("outcome_id")]
        public int OutcomeId { get; set; }

        [JsonProperty("outcome_name")]
        public string OutcomeName { get; set; }

        [JsonProperty("outcome_param")]
        public string OutcomeParam { get; set; }

        [JsonProperty("outcome_perc_stat")]
        public double OutcomePercStat { get; set; }

        [JsonProperty("outcome_short_name")]
        public string OutcomeShortName { get; set; }

        [JsonProperty("outcome_tag")]
        public object OutcomeTag { get; set; }

        [JsonProperty("outcome_tl_header_name")]
        public object OutcomeTlHeaderName { get; set; }

        [JsonProperty("outcome_tl_left_name")]
        public object OutcomeTlLeftName { get; set; }

        [JsonProperty("outcome_type_id")]
        public int OutcomeTypeId { get; set; }

        [JsonProperty("outcome_visible")]
        public bool OutcomeVisible { get; set; }

        [JsonProperty("participant_number")]
        public int? ParticipantNumber { get; set; }
    }

    public class BaseEvent_favbet
    {

        [JsonProperty("event_id")]
        public int EventId { get; set; }

        [JsonProperty("market_has_param")]
        public bool MarketHasParam { get; set; }

        [JsonProperty("market_id")]
        public int MarketId { get; set; }

        [JsonProperty("market_name")]
        public string MarketName { get; set; }

        [JsonProperty("market_order")]
        public string MarketOrder { get; set; }

        [JsonProperty("market_suspend")]
        public bool MarketSuspend { get; set; }

        [JsonProperty("market_template_id")]
        public int MarketTemplateId { get; set; }

        [JsonProperty("market_template_view_id")]
        public int MarketTemplateViewId { get; set; }

        [JsonProperty("market_template_view_schema")]
        public object MarketTemplateViewSchema { get; set; }

        [JsonProperty("market_template_weigh")]
        public int MarketTemplateWeigh { get; set; }

        [JsonProperty("outcomes")]
        public IList<Outcome> Outcomes { get; set; }

        [JsonProperty("result_type_id")]
        public int ResultTypeId { get; set; }

        [JsonProperty("result_type_name")]
        public string ResultTypeName { get; set; }

        [JsonProperty("result_type_short_name")]
        public string ResultTypeShortName { get; set; }

        [JsonProperty("result_type_weigh")]
        public int ResultTypeWeigh { get; set; }

        [JsonProperty("service_id")]
        public int ServiceId { get; set; }

        [JsonProperty("sport_id")]
        public int SportId { get; set; }
    }

    public class RequestEvent_favbet
    {

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("jsonrpc")]
        public string Jsonrpc { get; set; }

        [JsonProperty("result")]
        public IList<BaseEvent_favbet> Result { get; set; }
    }


}
