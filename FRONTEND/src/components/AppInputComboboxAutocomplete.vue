<template>
    <div>
        <b-form-group :id="groupId" v-bind="$attrs">
            <template v-slot:label>{{ labelStr }} <span v-if="isRequired" class="required"></span></template>
            <div v-if="isAllowEdit">
                <v-select :label="optionsLabel" append-to-body :options="options" :value="selectedValue" :filterable="true" @input="setSelected" @search="searchData" v-bind="$attrs"></v-select>
                <b-form-invalid-feedback :id="feedbackId" :state="state">
                    {{ errorMessage }}
                </b-form-invalid-feedback>
            </div>
            <div v-else>
                {{ selectedValue }}
            </div>
        </b-form-group>
    </div>
</template>
<script>
export default {
    props: ["model", "name", "selected", "errorMessage", "options", "state", "isEditable", "optionsLabel", "optionsKey", "change", "label", "isRequired", "onSelectMethod", "isSelectMethodExists"],
    data() {
        return {
            labelStr: this.label,
            value:"",
            selectedValue: this.selected,
            inputModel: this.model,
            groupId : "fieldset-" + name,
            feedbackId : "input-" + name + "-live-feedback",
        }
    }, 
    methods: {
        setSelected(value) {
            if (!this.isSelectMethodExists) {
                this.inputModel = value != null ? value[this.optionsKey] : null;
                this.selectedValue = value != null ? value[this.optionsLabel] : null;
                this.$emit('update:model', this.inputModel);
                this.$emit("change");
            } else {
                this.$emit("onSelectMethod", value);
            }
        },
        searchData(search, loading) {
            this.$emit("input", search);
        }
    },
    computed : {
        isAllowEdit : {
            get: function() {
                if (this.isEditable != undefined) return this.isEditable;
                else return true;
            }
        }
    },
    watch: {
        model(newValue) {
            this.inputModel = newValue;
            if (newValue == null || newValue == '') {
                this.selectedValue = '';
            }
        },
        selected(newValue) {
            this.selectedValue = newValue;
        }
    }
}
</script>
<style lang="scss" scoped>
.input-group-text {
    font-size: 0.7rem !important;
}
/deep/.vs__dropdown-toggle {
    background: #ffffff !important;
}
</style>